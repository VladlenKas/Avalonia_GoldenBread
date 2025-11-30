using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.DetailsPanels
{
    public enum PanelMode
    {
        Hidden,
        View,
        Edit,
        Create
    }

    public abstract class DetailsPanelViewModel<TEntity> : ReactiveValidationObject
    where TEntity : class, new()
    {
        // ==== Reactive Props ====
        [Reactive] public PanelMode Mode { get; set; }
        [Reactive] public TEntity? OriginalEntity { get; protected set; }
        [Reactive] public TEntity? EditableEntity { get; protected set; }

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
        protected DetailsPanelViewModel()
        {
            Mode = PanelMode.Hidden;
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
                        await SaveEntityAsync(EditableEntity);
                        OriginalEntity = EditableEntity;
                        Mode = Mode == PanelMode.Create ? PanelMode.Hidden : PanelMode.View;
                    }
                }, canSave);

            DeleteCommand = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    if (OriginalEntity != null)
                    {
                        await DeleteEntityAsync(OriginalEntity);
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

        // ==== Abstracts Methods ====
        protected abstract TEntity CloneEntity(TEntity entity);
        protected abstract bool ValidateEntity(TEntity entity);
        protected abstract Task SaveEntityAsync(TEntity entity);
        protected abstract Task DeleteEntityAsync(TEntity entity);
    }
}
