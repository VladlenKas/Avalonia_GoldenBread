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
    public abstract class PageViewModelBase<T> : ValidatableViewModelBase
    where T : class
    {
        // ==== Fields ====
        protected readonly SourceCache<T, int> _sourceCache;
        private readonly ReadOnlyObservableCollection<T> _items;
        private readonly Func<T, int> _keySelector;


        // ==== Props ====
        // Публичные для доступа из CrudCommandsManager
        public readonly DetailPanelManager PanelManager;
        private readonly CrudCommandsManager<T> _commandsManager;

        // Data
        [Reactive] public string SearchText { get; set; }
        [Reactive] public T SelectedItem { get; set; }
        [Reactive] public bool IsLoading { get; set; }
        [Reactive] private int? SelectedItemKey { get; set; }
        public ReadOnlyObservableCollection<T> Items => _items;

        // from Crud Commands Manager
        public bool IsDetailPanelOpen
        {
            get => PanelManager.IsDetailPanelOpen;
            set => PanelManager.IsDetailPanelOpen = value;
        }
        public PanelMode CurrentMode
        {
            get => PanelManager.CurrentMode;
            set => PanelManager.CurrentMode = value;
        }
        public string ModeTitle => PanelManager.ModeTitle;
        public bool ShowViewButtons => PanelManager.ShowViewButtons;
        public bool ShowEditButtons => PanelManager.ShowEditButtons;

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
            _keySelector = keySelector;
            _sourceCache = new SourceCache<T, int>(keySelector);
            PanelManager = new DetailPanelManager();

            // Initializing commands through the manager
            var canDelete = this.WhenAnyValue(x => x.SelectedItem)
                .Select(item => item != null && GetDeleteOptions());

            _commandsManager = new CrudCommandsManager<T>(this, service, canDelete);

            // Filtering
            var searchFilter = this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Select(text => CreateSearchFilter(text));

            var additionalFilters = GetAdditionalFilters();

            var combinedFilter = Observable.CombineLatest(
                searchFilter, additionalFilters,
                (search, additional) => new Func<T, bool>(item =>
                    search(item) && additional(item)));

            _sourceCache.Connect()
                .Filter(combinedFilter)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();

            ViewCommand = ReactiveCommand.Create<T>(_ =>
            {
                PanelManager.OpenView();
            });

            RefreshCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                IsLoading = true;
                try { await LoadDataAsync(); }
                finally { IsLoading = false; }
            });

            // Subscribing to panel changes
            PanelManager.WhenAnyValue(
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

            _sourceCache.Connect()
                .WhereReasonsAre(ChangeReason.Update, ChangeReason.Refresh)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    if (SelectedItemKey.HasValue && CurrentMode == PanelMode.View)
                    {
                        var restoredItem = Items.FirstOrDefault(item =>
                            _keySelector(item) == SelectedItemKey.Value);

                        if (restoredItem != null)
                        {
                            // Временно отключаем подписку, чтобы избежать цикла
                            SelectedItem = restoredItem;
                        }
                    }
                });

            // Основная подписка на изменение SelectedItem
            this.WhenAnyValue(x => x.SelectedItem)
                .Where(item => item != null)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(item =>
                {
                    SelectedItemKey = _keySelector(item);

                    if (CurrentMode == PanelMode.View)
                        CopyToEditFields(item);
                });
        }


        // ==== Virtual Methods ====
        protected virtual bool GetDeleteOptions() => true;
        protected virtual string GetSearchableText(T item) => item?.ToString() ?? "";
        protected virtual IObservable<Func<T, bool>> GetAdditionalFilters()
            => Observable.Return<Func<T, bool>>(_ => true);


        // ==== Abstract Methods ====
        public abstract Task LoadDataAsync();
        public abstract Task OnSaveAsync();
        public abstract Task OnDeleteAsync();
        protected abstract void CopyToEditFields(T item);
        public abstract void ClearEditFields();


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
    }
}
