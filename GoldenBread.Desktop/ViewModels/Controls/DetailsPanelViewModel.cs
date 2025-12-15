using GoldenBread.Desktop.Bases;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.Services.Crud;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoldenBread.Desktop.ViewModels.Controls
{
    public enum PanelMode
    {
        Hidden,
        View,
        Edit,
        Create
    }

    public class DetailsPanelViewModel<T> : ViewModelValidationBase, IDetailsPanelViewModel
        where T : class, new()
    {
        // ==== Services ====
        private readonly ICrudService<T> _crudService;

        // ==== Reactive Props ====
        [Reactive] public PanelMode Mode { get; set; }
        [Reactive] public T? OriginalEntity { get; protected set; }
        [Reactive] public T? EditableEntity { get; protected set; }
        [Reactive] public string HeaderTitle { get; private set; }

        // ==== OAPHs ====
        private ObservableAsPropertyHelper<bool> _isViewMode;
        private ObservableAsPropertyHelper<bool> _isEditMode;
        private ObservableAsPropertyHelper<bool> _isCreateMode;
        private ObservableAsPropertyHelper<bool> _isOpen;

        // ==== Props ====
        public bool IsViewMode => _isViewMode.Value;
        public bool IsEditMode => _isEditMode.Value;
        public bool IsCreateMode => _isCreateMode.Value;
        public bool IsOpen => _isOpen.Value;

        // ==== Commands ====
        public ReactiveCommand<Unit, Unit> EditCommand { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }

        // ==== Events ====
        public event Action<T>? OnEntitySaved;
        public event Action<int>? OnEntityDeleted;

        // ==== Deigner ====
        public DetailsPanelViewModel(ICrudService<T> crudService)
        {
            Mode = PanelMode.Hidden;
            _crudService = crudService;
            Initialize();
        }

        // ==== Methods ====
        private void Initialize()
        {
            // OAPHs
            _isViewMode = this
                .WhenAnyValue(x => x.Mode)
                .Select(m => m == PanelMode.View)
                .ToProperty(this, x => x.IsViewMode);

            _isEditMode = this
                .WhenAnyValue(x => x.Mode)
                .Select(m => m == PanelMode.Edit)
                .ToProperty(this, x => x.IsEditMode);

            _isCreateMode = this
                .WhenAnyValue(x => x.Mode)
                .Select(m => m == PanelMode.Create)
                .ToProperty(this, x => x.IsCreateMode);

            _isOpen = this
                .WhenAnyValue(x => x.Mode)
                .Select(m => m != PanelMode.Hidden)
                .ToProperty(this, x => x.IsOpen);

            // Optins for commands
            var canEdit = this
                .WhenAnyValue(x => x.Mode)
                .Select(m => m == PanelMode.View);

            var canSave = this
                .WhenAnyValue(x => x.EditableEntity)
                .Select(m => m != null && ValidateEntity(m!));

            var canDelete = this
                .WhenAnyValue(x => x.Mode)
                .Select(m => m == PanelMode.View);

            var canCancel = this
                .WhenAnyValue(x => x.Mode)
                .Select(m => m == PanelMode.Edit);

            // Commands
            EditCommand = ReactiveCommand.Create(
                () =>
                {
                    if (OriginalEntity != null)
                    {
                        EditableEntity = CloneEntity(OriginalEntity);
                        Mode = PanelMode.Edit;
                    }
                }, canEdit);

            SaveCommand = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    if (EditableEntity != null)
                    {
                        var ok = await SaveEntityAsync(EditableEntity);
                        if (!ok) return; 

                        OriginalEntity = EditableEntity;
                        OnEntitySaved?.Invoke(EditableEntity);
                        Mode = PanelMode.Hidden;
                    }
                }, canSave);

            DeleteCommand = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    if (OriginalEntity != null)
                    {
                        var ok = await DeleteEntityAsync(OriginalEntity);
                        if (!ok) return;

                        var entityId = GetEntityId(OriginalEntity);
                        OnEntityDeleted?.Invoke(entityId);
                        Mode = PanelMode.Hidden;
                    }
                }, canDelete);

            CancelCommand = ReactiveCommand.Create(
                () =>
                {
                    if (OriginalEntity != null)
                    {
                        EditableEntity = CloneEntity(OriginalEntity);
                        Mode = PanelMode.View;
                    }
                }, canCancel);

            CloseCommand = ReactiveCommand.Create(
                () =>
                {
                    Mode = PanelMode.Hidden;
                });

            // Props for ui control
            this.WhenAnyValue(x => x.Mode)
                .Subscribe(mode =>
                {
                    HeaderTitle = mode switch
                    {
                        PanelMode.View => "Просмотр",
                        PanelMode.Edit => "Редактирование",
                        PanelMode.Create => "Добавение",
                        _ => string.Empty
                    };
                });
        }

        // For panel
        public void ShowDetails(T entity)
        {
            OriginalEntity = entity;
            EditableEntity = CloneEntity(OriginalEntity);
            Mode = PanelMode.View;
        }

        public void ShowCreate()
        {
            OriginalEntity = null;
            EditableEntity = new T();
            Mode = PanelMode.Create;
        }

        // For crud
        private Func<T, int> _keySelector;

        public void SetKeySelector(Func<T, int> keySelector)
        {
            _keySelector = keySelector;
        }

        private int GetEntityId(T entity)
        {
            return _keySelector?.Invoke(entity) ?? 0;
        }

        // ==== Methods for commands ====
        private T CloneEntity(T entity) => _crudService.Clone(entity);
        private bool ValidateEntity(T entity) => _crudService.Validate(entity);
        private Task<bool> SaveEntityAsync(T entity) => _crudService.SaveAsync(entity);
        private Task<bool> DeleteEntityAsync(T entity) => _crudService.DeleteAsync(entity);

        // ==== Interfaces ====
        ICommand? IDetailsPanelViewModel.CloseCommand => CloseCommand;
    }
}
