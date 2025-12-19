using AutoMapper;
using DynamicData;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.Managers;
using GoldenBread.Desktop.Mappers;
using GoldenBread.Desktop.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Bases
{
    public abstract class PageViewModelBase<TEntity> : 
        ViewModelValidationBase, IDrawerViewModel
        where TEntity : class
    {
        // ==== Fields ====
        protected readonly IMapper<TEntity> _mapper;
        protected readonly IService<TEntity> _service;
        protected readonly SourceCache<TEntity, int> _sourceCache;
        private readonly Func<TEntity, int> _keySelector;
        private TEntity? _editingEntity;

        // ==== Managers ====
        public readonly CrudDrawerManager DrawerManager;

        // ==== Properties ====
        public ReadOnlyObservableCollection<TEntity> Items { get; }
        [Reactive] public TEntity? SelectedItem { get; set; }

        public bool IsDrawerVisible
        {
            get => DrawerManager.IsOpen;
            set => DrawerManager.IsOpen = value;
        }

        public DrawerMode CurrentMode
        {
            get => DrawerManager.CurrentMode;
            set => DrawerManager.CurrentMode = value;
        }

        public string DrawerTitle => DrawerManager.ModeTitle;
        public bool ShowViewButtons => DrawerManager.ShowViewButtons;
        public bool ShowEditButtons => DrawerManager.ShowEditButtons;

        // ==== Commands ====
        public ReactiveCommand<Unit, Unit> AddCommand { get; set; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; set; }

        // ==== Constructor ====
        protected PageViewModelBase(
            Func<TEntity, int> keySelector,
            IService<TEntity> service,
            IMapper<TEntity> mapper)
        {
            _keySelector = keySelector;
            _service = service;
            _mapper = mapper;
            _sourceCache = new SourceCache<TEntity, int>(keySelector);
            DrawerManager = new CrudDrawerManager();

            _sourceCache.Connect()
                .Bind(out var items)
                .Subscribe();

            Items = items;

            InitializeCommands();
            SetupSubscriptions();

            CurrentMode = DrawerMode.None;
        }


        // ==== Command Creation ====
        private void InitializeCommands()
        {
            var canEdit = this.WhenAnyValue(
                    x => x.SelectedItem,
                    x => x.CurrentMode)
                .Select(t => t.Item1 != null && t.Item2 == DrawerMode.View);
            EditCommand = ReactiveCommand.Create(ExecuteEdit, canEdit);

            var canSave = this.WhenAnyValue(x => x.CurrentMode)
                .Select(mode => mode == DrawerMode.Edit || mode == DrawerMode.Add)
                .CombineLatest(
                    ValidationContext.Valid,
                    (modeOk, isValid) => modeOk && isValid);
            SaveCommand = ReactiveCommand.CreateFromTask(ExecuteSaveAsync, canSave);

            var canDelete = this.WhenAnyValue(
                    x => x.SelectedItem,
                    x => x.CurrentMode)
                .Select(t => t.Item1 != null &&
                            t.Item2 == DrawerMode.View &&
                            CanDelete(t.Item1));
            DeleteCommand = ReactiveCommand.CreateFromTask(ExecuteDeleteAsync, canDelete);

            AddCommand = ReactiveCommand.Create(ExecuteAdd);
            RefreshCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
            CloseCommand = ReactiveCommand.Create(ExecuteClose);
            CancelCommand = ReactiveCommand.Create(ExecuteCancel);
        }


        // ==== Subscriptions ====
        private void SetupSubscriptions()
        {
            // При выборе элемента переходим в режим просмотра
            this.WhenAnyValue(x => x.SelectedItem)
                .Where(item => item != null)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                { 
                    DrawerManager.OpenView();
                    _mapper.MapEntityToViewModel(SelectedItem!, this);
                });

            // Подписываемся на обновления UI панели
            DrawerManager.WhenAnyValue(
                x => x.IsOpen,
                x => x.CurrentMode)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(IsDrawerVisible));
                this.RaisePropertyChanged(nameof(CurrentMode));
                this.RaisePropertyChanged(nameof(DrawerTitle));
                this.RaisePropertyChanged(nameof(ShowViewButtons));
                this.RaisePropertyChanged(nameof(ShowEditButtons));
            });
        }


        // ==== Command Handlers ====
        private void ExecuteAdd()
        {
            DrawerManager.OpenAdd();
            _editingEntity = null;
            ClearEditFields();
            DeactivateValidation();
        }

        private void ExecuteEdit()
        {
            if (SelectedItem == null) return;

            DrawerManager.OpenEdit();
            _editingEntity = _service.Clone(SelectedItem);
            _mapper.MapEntityToViewModel(_editingEntity, this);
            DeactivateValidation();
        }

        private async Task ExecuteSaveAsync()
        {
            if (!Validate()) return;

            try
            {
                bool isNew = CurrentMode == DrawerMode.Add;
                var entity = _mapper.MapEntityFromViewModel(this, _editingEntity);
                var result = await _service.SaveAsync(entity, isNew);

                if (result.IsSuccess)
                {
                    _sourceCache.AddOrUpdate(result.Data);
                    SelectedItem = result.Data;
                    DrawerManager.OpenView();

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
            if (CurrentMode == DrawerMode.Edit && SelectedItem != null)
            {
                DrawerManager.OpenView();
                _mapper.MapEntityToViewModel(SelectedItem, this);
            }
            else if (CurrentMode == DrawerMode.Add)
            {
                DrawerManager.Close();
                ClearEditFields();
                SelectedItem = null;
            }

            DeactivateValidation();
        }

        private void ExecuteClose()
        {
            DrawerManager.Close();
            ClearEditFields();

            SelectedItem = null;
            DeactivateValidation();
        }

        private async Task ExecuteDeleteAsync()
        {
            if (SelectedItem == null) return;

            var confirmed = await MessageBoxHelper.ShowQuestionMessageBox(ValidationMessages.ConfirmDelete);

            if (!confirmed) return;

            var result = await _service.DeleteAsync(SelectedItem);

            if (result.IsSuccess)
            {
                _sourceCache.RemoveKey(_keySelector(SelectedItem));
                DrawerManager.Close();
                SelectedItem = null;

                await MessageBoxHelper.ShowOkMessageBox(result.Message);
            }
            else
            {
                await MessageBoxHelper.ShowErrorMessageBox(result.Message);
            }
        }

        protected void Initialize() => RefreshCommand.Execute().Subscribe();

        // ==== Abstract/Virtual Methods ====
        public abstract Task LoadDataAsync();
        protected abstract void ClearEditFields();
        protected virtual bool CanDelete(TEntity entity) => true;
    }

    // ==== Enums ====
    public enum DrawerMode
    {
        None,
        View,
        Edit,
        Add
    }
}

