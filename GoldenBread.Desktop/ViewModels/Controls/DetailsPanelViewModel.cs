using GoldenBread.Desktop.Services.Crud;
using GoldenBread.Desktop.ViewModels.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.Controls
{
    public enum PanelMode
    {
        Hidden,
        View,
        Edit,
        Create
    }

    public class DetailsPanelViewModel<TEntity> : ViewModelBase where TEntity : class, new()
    {
        // ==== Services ====
        private readonly ICrudService<TEntity> _crudService;

        // ==== Reactive Props ====
        [Reactive] public PanelMode Mode { get; set; }
        [Reactive] public TEntity? OriginalEntity { get; protected set; }
        [Reactive] public TEntity? EditableEntity { get; protected set; }
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


        // ==== Deigner ====
        public DetailsPanelViewModel(ICrudService<TEntity> crudService)
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
                        Mode = Mode == PanelMode.Create ? PanelMode.Hidden : PanelMode.View;
                    }
                }, canSave);

            DeleteCommand = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    if (OriginalEntity != null)
                    {
                        var ok = await DeleteEntityAsync(OriginalEntity);
                        if (!ok) return;

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

        public void ShowDetails(TEntity entity)
        {
            OriginalEntity = entity;
            EditableEntity = CloneEntity(OriginalEntity);
            Mode = PanelMode.View;
        }

        public void ShowCreate()
        {
            OriginalEntity = null;
            EditableEntity = new TEntity();
            Mode = PanelMode.Create;
        }

        // ==== Methods for commands ====
        private TEntity CloneEntity(TEntity entity) => _crudService.Clone(entity);
        private bool ValidateEntity(TEntity entity) => _crudService.Validate(entity);
        private Task<bool> SaveEntityAsync(TEntity entity) => _crudService.SaveAsync(entity);
        private Task<bool> DeleteEntityAsync(TEntity entity) => _crudService.DeleteAsync(entity);
    }
}
