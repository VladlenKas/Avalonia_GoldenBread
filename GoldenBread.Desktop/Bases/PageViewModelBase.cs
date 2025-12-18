using DynamicData;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services.Api;
using GoldenBread.Desktop.Services.Crud;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Bases
{
    public abstract class PageViewModelBase<TEntity> : ViewModelValidationBase
        where TEntity : class
    {
        // ==== Fields ====
        protected readonly ICrudService<TEntity> _crudService;
        protected readonly IApiService<TEntity> _apiService;
        protected readonly SourceCache<TEntity, int> _sourceCache;
        private readonly Func<TEntity, int> _keySelector;


        // ==== Properties ====
        public IObservable<IChangeSet<TEntity, int>> ItemsObservable { get; }
        public ReadOnlyObservableCollection<TEntity> Items { get; }
        [Reactive] public TEntity? SelectedItem { get; set; }
        [Reactive] public PanelMode CurrentMode { get; set; } = PanelMode.None;
        [Reactive] public string SearchText { get; set; } = string.Empty;
        [Reactive] public string PanelTitle { get; set; } = string.Empty;

        private TEntity? _editingEntity; // Временная копия для редактирования


        // ==== Computed Properties ====
        private readonly ObservableAsPropertyHelper<bool> _isViewMode;
        public bool IsViewMode => _isViewMode.Value;

        private readonly ObservableAsPropertyHelper<bool> _isEditMode;
        public bool IsEditMode => _isEditMode.Value;

        private readonly ObservableAsPropertyHelper<bool> _isAddMode;
        public bool IsAddMode => _isAddMode.Value;

        private readonly ObservableAsPropertyHelper<bool> _isEditOrAddMode;
        public bool IsEditOrAddMode => _isEditOrAddMode.Value;

        private readonly ObservableAsPropertyHelper<bool> _isPanelVisible;
        public bool IsPanelVisible => _isPanelVisible.Value;


        // ==== Commands ====
        public ReactiveCommand<Unit, Unit> AddCommand { get; set; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }


        // ==== Constructor ====
        protected PageViewModelBase(
            Func<TEntity, int> keySelector,
            IApiService<TEntity> apiService,
            ICrudService<TEntity> crudService)
        {
            _keySelector = keySelector;
            _apiService = apiService;
            _crudService = crudService;
            _sourceCache = new SourceCache<TEntity, int>(keySelector);

            _isViewMode = this.WhenAnyValue(x => x.CurrentMode)
            .Select(mode => mode == PanelMode.View)
            .ToProperty(this, x => x.IsViewMode);

            _isEditMode = this.WhenAnyValue(x => x.CurrentMode)
                .Select(mode => mode == PanelMode.Edit)
                .ToProperty(this, x => x.IsEditMode);

            _isAddMode = this.WhenAnyValue(x => x.CurrentMode)
                .Select(mode => mode == PanelMode.Add)
                .ToProperty(this, x => x.IsAddMode);

            _isEditOrAddMode = this.WhenAnyValue(x => x.CurrentMode)
                .Select(mode => mode == PanelMode.Edit || mode == PanelMode.Add)
                .ToProperty(this, x => x.IsEditOrAddMode);

            _isPanelVisible = this.WhenAnyValue(x => x.CurrentMode)
                .Select(mode => mode != PanelMode.None)
                .ToProperty(this, x => x.IsPanelVisible);

            ItemsObservable = _sourceCache.Connect();
            ItemsObservable
                .Bind(out var items)
                .Subscribe();
            Items = items;

            CreateCommands();
            SetupSubscriptions();
        }


        // ==== Command Creation ====
        private void CreateCommands()
        {
            // Команда добавления
            AddCommand = ReactiveCommand.Create(ExecuteAdd);

            // Команда редактирования
            var canEdit = this.WhenAnyValue(x => x.SelectedItem)
                .Select(item => item != null && CurrentMode == PanelMode.View);
            EditCommand = ReactiveCommand.Create(ExecuteEdit, canEdit);

            // Команда сохранения
            var canSave = this.WhenAnyValue(x => x.IsDirty)
                .Select(dirty => !dirty)
                .Merge(ValidationContext.Valid)
                .CombineLatest(
                    this.WhenAnyValue(x => x.CurrentMode),
                    (isValid, mode) => isValid && (mode == PanelMode.Edit || mode == PanelMode.Add));

            SaveCommand = ReactiveCommand.CreateFromTask(ExecuteSaveAsync, canSave);

            // Команда отмены
            var canCancel = this.WhenAnyValue(x => x.CurrentMode)
                .Select(mode => mode == PanelMode.Edit || mode == PanelMode.Add);
            CancelCommand = ReactiveCommand.Create(ExecuteCancel, canCancel);

            // Команда удаления
            var canDelete = this.WhenAnyValue(
                    x => x.SelectedItem,
                    x => x.CurrentMode)
                .Select(tuple => tuple.Item1 != null &&
                               tuple.Item2 == PanelMode.View &&
                               CanDelete(tuple.Item1));
            DeleteCommand = ReactiveCommand.CreateFromTask(ExecuteDeleteAsync, canDelete);
        }


        // ==== Subscriptions ====
        private void SetupSubscriptions()
        {
            // При выборе элемента переходим в режим просмотра
            this.WhenAnyValue(x => x.SelectedItem)
                .Where(item => item != null)
                .Subscribe(_ => SwitchToViewMode());

            // Очистка валидации при смене режима
            this.WhenAnyValue(x => x.CurrentMode)
                .Subscribe(_ => DeactivateValidation());
        }


        // ==== Command Handlers ====
        private void ExecuteAdd()
        {
            CurrentMode = PanelMode.Add;
            PanelTitle = GetAddTitle();

            _editingEntity = default;
            ClearEditFields();
            DeactivateValidation();
        }

        private void ExecuteEdit()
        {
            if (SelectedItem == null) return;

            CurrentMode = PanelMode.Edit;
            PanelTitle = GetEditTitle();

            // Создаем копию для редактирования
            _editingEntity = _crudService.Clone(SelectedItem);

            // Копируем данные в реактивные свойства
            _crudService.MapToViewModel(_editingEntity, this);
            DeactivateValidation();
        }

        private async Task ExecuteSaveAsync()
        {
            if (!Validate()) return;

            try
            {
                bool isNew = CurrentMode == PanelMode.Add;

                // Создаем/обновляем сущность из VM свойств
                var entity = _crudService.MapFromViewModel(this, _editingEntity);

                // Сохраняем через сервис
                var result = await _crudService.SaveAsync(entity, isNew);

                if (result.IsSuccess)
                {
                    // Обновляем кэш
                    _sourceCache.AddOrUpdate(result.Data);

                    // Переходим в режим просмотра
                    SelectedItem = result.Data;
                    SwitchToViewMode();

                    await MessageBoxHelper.ShowOkMessageBox(result.Message);
                }
                else
                {
                    await MessageBoxHelper.ShowErrorMessageBox(result.Message);
                }
            }
            catch (Exception ex)
            {
                await MessageBoxHelper.ShowErrorMessageBox($"Ошибка сохранения: {ex.Message}");
            }
            finally
            {
                DeactivateValidation();
            }
        }

        private void ExecuteCancel()
        {
            if (CurrentMode == PanelMode.Edit && SelectedItem != null)
            {
                // Возвращаемся к просмотру оригинальных данных
                SwitchToViewMode();
            }
            else if (CurrentMode == PanelMode.Add)
            {
                // Просто скрываем панель
                CurrentMode = PanelMode.None;
                ClearEditFields();
            }

            DeactivateValidation();
        }

        private async Task ExecuteDeleteAsync()
        {
            if (SelectedItem == null) return;

            var confirmed = await MessageBoxHelper.ShowQuestionMessageBox(
                GetDeleteConfirmationMessage());

            if (!confirmed) return;

            var result = await _crudService.DeleteAsync(SelectedItem);

            if (result.IsSuccess)
            {
                _sourceCache.RemoveKey(_keySelector(SelectedItem));
                CurrentMode = PanelMode.None;
                SelectedItem = default;

                await MessageBoxHelper.ShowOkMessageBox(result.Message);
            }
            else
            {
                await MessageBoxHelper.ShowErrorMessageBox(result.Message);
            }
        }


        // ==== Helper Methods ====
        private void SwitchToViewMode()
        {
            if (SelectedItem == null) return;

            CurrentMode = PanelMode.View;
            PanelTitle = GetViewTitle();

            // Показываем данные из оригинальной модели
            _crudService.MapToViewModel(SelectedItem, this);
        }


        // ==== Abstract/Virtual Methods ====
        protected abstract void ClearEditFields();
        protected virtual bool CanDelete(TEntity entity) => true;
        protected virtual string GetViewTitle() => "Просмотр";
        protected virtual string GetEditTitle() => "Редактирование";
        protected virtual string GetAddTitle() => "Добавление";
        protected virtual string GetDeleteConfirmationMessage() =>
            "Вы действительно хотите удалить выбранный элемент?";

        public abstract Task LoadDataAsync();
    }


    // ==== Enums ====
    public enum PanelMode
    {
        None,
        View,
        Edit,
        Add
    }
}

