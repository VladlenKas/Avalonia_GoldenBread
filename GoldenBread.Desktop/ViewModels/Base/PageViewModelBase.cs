using Avalonia.Controls;
using Avalonia.Controls.Documents;
using DynamicData;
using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.Managers;
using GoldenBread.Desktop.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.Base
{
    public abstract class PageViewModelBase<T> : ValidatableViewModelBase, IDetailPanelViewModel
    where T : class
    {
        // ==== Fields ====
        protected readonly SourceCache<T, int> _sourceCache;
        private readonly ReadOnlyObservableCollection<T> _items;
        private readonly DetailPanelManager _panelManager; 
        private readonly CrudCommandsManager<T> _commandsManager;


        // ==== Props ====
        // Data
        [Reactive] public string SearchText { get; set; }
        [Reactive] public T SelectedItem { get; set; }
        [Reactive] public bool IsLoading { get; set; }
        public ReadOnlyObservableCollection<T> Items => _items;

        // from Crud Commands Manager
        public bool IsDetailPanelOpen
        {
            get => _panelManager.IsDetailPanelOpen;
            set => _panelManager.IsDetailPanelOpen = value;
        }
        public PanelMode CurrentMode
        {
            get => _panelManager.CurrentMode;
            set => _panelManager.CurrentMode = value;
        }
        public string ModeTitle => _panelManager.ModeTitle;
        public bool ShowViewButtons => _panelManager.ShowViewButtons;
        public bool ShowEditButtons => _panelManager.ShowEditButtons;

        // Commands required for binding on pages !!!
        public bool CanAdd => _commandsManager.CanAdd;
        public bool CanEdit => _commandsManager.CanEdit;
        public bool CanDelete => _commandsManager.CanDelete;
        public ReactiveCommand<Unit, Unit>? AddCommand => _commandsManager.AddCommand;
        public ReactiveCommand<Unit, Unit>? SaveCommand => _commandsManager.SaveCommand;
        public ReactiveCommand<Unit, Unit>? EnterEditModeCommand => _commandsManager.EnterEditModeCommand;
        public ReactiveCommand<Unit, Unit>? DeleteFromPanelCommand => _commandsManager.DeleteFromPanelCommand;
        public ReactiveCommand<Unit, Unit> CancelCommand => _commandsManager.CancelCommand;

        public ReactiveCommand<T, Unit> ViewCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }


        // ==== Designer ====
        protected PageViewModelBase(Func<T, int> keySelector, AuthorizationService service) 
            : base()
        {
            _sourceCache = new SourceCache<T, int>(keySelector);
            _panelManager = new DetailPanelManager();

            // Initializing commands through the manager
            var canDelete = this.WhenAnyValue(x => x.SelectedItem)
                .Select(item => item != null && GetDeleteOptions());

            _commandsManager = new CrudCommandsManager<T>(
                _panelManager,
                service,
                saveAction: async () => await OnSaveAsync(),
                deleteAction: async () => await OnDeleteAsync(),
                refreshAction: async () => await LoadDataAsync(),
                canDelete: canDelete,
                viewModel: this,
                detailVm: this
            );

            // Filtering
            var searchFilter = this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Select(text => CreateSearchFilter(text));

            var additionalFilters = GetAdditionalFilters();

            var combinedFilter = Observable.CombineLatest(
                searchFilter,
                additionalFilters,
                (search, additional) => new Func<T, bool>(item =>
                    search(item) && additional(item)));

            _sourceCache.Connect()
                .Filter(combinedFilter)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();

            ViewCommand = ReactiveCommand.Create<T>(item =>
            {
                SelectedItem = item;
                _panelManager.OpenView();
            });

            RefreshCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                IsLoading = true;
                try { await LoadDataAsync(); }
                finally { IsLoading = false; }
            });

            // Subscribing to panel changes
            _panelManager.WhenAnyValue(
                x => x.IsDetailPanelOpen,
                x => x.CurrentMode)
                .Subscribe(_ =>
                {
                    this.RaisePropertyChanged(nameof(IsDetailPanelOpen));
                    this.RaisePropertyChanged(nameof(CurrentMode));
                    this.RaisePropertyChanged(nameof(ModeTitle));
                    this.RaisePropertyChanged(nameof(ShowViewButtons));
                    this.RaisePropertyChanged(nameof(ShowEditButtons));
                });

            // Subscribing to item changed
            this.WhenAnyValue(x => x.SelectedItem)
                .Where(item => item != null)
                .Subscribe(item => CopyToEditFields(item));

            this.WhenAnyValue(x => x.CurrentMode)
                .Subscribe(mode =>
                {
                    if (mode == PanelMode.View && SelectedItem != null)
                        CopyToEditFields(SelectedItem);
                    else if (mode == PanelMode.Add)
                        ClearEditFields();
                });
        }


        // ==== Virtual Methods ====
        protected virtual bool GetDeleteOptions() => true;
        protected virtual string GetSearchableText(T item) => item?.ToString() ?? "";
        protected virtual IObservable<Func<T, bool>> GetAdditionalFilters()
            => Observable.Return<Func<T, bool>>(_ => true);


        // ==== Abstract Methods ====
        protected abstract Task LoadDataAsync();
        protected abstract Task OnSaveAsync();
        protected abstract Task OnDeleteAsync();
        protected abstract void CopyToEditFields(T item);
        protected abstract void ClearEditFields();


        // ==== Helper methods ====
        protected void Initialize() => RefreshCommand.Execute().Subscribe();
        protected void AddOrUpdateItem(T item) => _sourceCache.AddOrUpdate(item);
        protected void RemoveItem(T item) => _sourceCache.Remove(item);
        private Func<T, bool> CreateSearchFilter(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return _ => true;
            var lower = text.ToLower();
            return item => GetSearchableText(item).ToLower().Contains(lower);
        }
        public void ResetOnCancel()
        {
            if (CurrentMode == PanelMode.Edit && SelectedItem != null)
            {
                CopyToEditFields(SelectedItem);
                CurrentMode = PanelMode.View;
            }
            else
            {
                ClearEditFields();
                IsDetailPanelOpen = false;
            }

            this.DeactivateValidation();
        }
        void IDetailPanelViewModel.ClearEditFields() => ClearEditFields();
    }
}
