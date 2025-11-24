using Avalonia.Controls;
using Avalonia.Controls.Documents;
using DynamicData;
using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Interfaces;
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
    public abstract class PageViewModelBase<T> : ReactiveValidationObject, 
        IDetailPanelViewModel where T : class
    {
        // == Fields ==
        protected readonly AuthorizationService? _service;
        public readonly SourceCache<T, int> _sourceCache;
        private readonly ReadOnlyObservableCollection<T> _items;


        // == Props ==
        [Reactive] public string SearchText { get; set; }
        [Reactive] public T SelectedItem { get; set; }
        [Reactive] public bool IsLoading { get; set; }
        [Reactive] public bool IsFilterMenuOpen { get; set; }

        // Panel view/edit
        [Reactive] public bool IsDetailPanelOpen { get; set; }
        [Reactive] public PanelMode CurrentMode { get; set; }
        public string ModeTitle => CurrentMode switch
        {
            PanelMode.Add => "Добавление",
            PanelMode.Edit => "Редактирование",
            _ => "Просмотр"
        };

        public bool ShowViewButtons => CurrentMode == PanelMode.View;
        public bool ShowEditButtons => CurrentMode != PanelMode.View;

        public ReadOnlyObservableCollection<T> Items => _items;

        // Access rights 
        public bool CanAdd { get; }
        public bool CanEdit { get; }
        public bool CanDelete { get; }


        // == Commands ==
        public ReactiveCommand<Unit, Unit>? AddCommand { get; }
        public ReactiveCommand<Unit, Unit>? SaveCommand { get; }    // Btn save inside panel
        public ReactiveCommand<T, Unit> ViewCommand { get; }    // For Double tap
        public ReactiveCommand<Unit, Unit>? EnterEditModeCommand { get; }   // Btn edit inside panel
        public ReactiveCommand<Unit, Unit>? DeleteFromPanelCommand { get; } // Btn delete inside panel
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }   // Btn cansel inside panel
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Unit, bool> ToggleFilterCommand { get; }

        
        // == Designer ==
        protected PageViewModelBase(Func<T, int> keySelector, 
            AuthorizationService service)
        {
            _sourceCache = new SourceCache<T, int>(keySelector);
            _service = service;

            // For Designer
            if (Design.IsDesignMode)
            {
                CanAdd = true;
                CanEdit = true;
                CanDelete = true;

                ViewCommand = ReactiveCommand.Create<T>(_ => { });
                CancelCommand = ReactiveCommand.Create(() => { });
                RefreshCommand = ReactiveCommand.CreateFromTask(async () => { });
                ToggleFilterCommand = ReactiveCommand.Create(() => false);

                _sourceCache.Connect()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Bind(out _items)
                    .Subscribe();

                return; 
            }

            // 1. Distribution of rights
            CanAdd = GetPermission(Permission.Add);
            CanEdit = GetPermission(Permission.Edit);
            CanDelete = GetPermission(Permission.Delete);

            this.WhenAnyValue(x => x.CurrentMode)
                .Subscribe(_ =>
                {
                    this.RaisePropertyChanged(nameof(ModeTitle));
                    this.RaisePropertyChanged(nameof(ShowViewButtons));
                    this.RaisePropertyChanged(nameof(ShowEditButtons));
                });

            // 2. Load Commands
            if (CanAdd)
            {
                AddCommand = ReactiveCommand.CreateFromTask(OnAddAsync);
            }

            if (CanEdit)
            {
                EnterEditModeCommand = ReactiveCommand.Create(EnterEditMode);
                SaveCommand = ReactiveCommand.CreateFromTask(SaveChangesAsync);
            }

            if (CanDelete)  
            {
                var canDelete = this.WhenAnyValue(x => x.SelectedItem)
                    .Select(item => item != null && CanDeleteOptions());
                DeleteFromPanelCommand = ReactiveCommand.CreateFromTask(DeleteFromPanelAsync, canDelete);
            }

            CancelCommand = ReactiveCommand.Create(CloseOrCancelEdit);
            ViewCommand = ReactiveCommand.Create<T>(OpenViewPanel);
            ToggleFilterCommand = ReactiveCommand.Create(() =>
                IsFilterMenuOpen = !IsFilterMenuOpen);
            RefreshCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                IsLoading = true;
                try
                {
                    await LoadDataAsync();
                }
                finally
                {
                    IsLoading = false;
                }
            });

            // 3. Load Filters
            // Base Filter Search
            var searchFilter = this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .ObserveOn(RxApp.TaskpoolScheduler) // Run in the background stream
                .Select(text => CreateSearchFilter(text))
                .ObserveOn(RxApp.MainThreadScheduler); // Return in UI-stream 

            // Additional filters
            var additionalFilters = GetAdditionalFilters();

            // Combining filters
            var combinedFilter = Observable
                .CombineLatest(searchFilter, additionalFilters,
                    (search, additional) => new Func<T, bool>(item =>
                        search(item) && additional(item)))
                .DistinctUntilChanged();

            // Reactive flow with auto async
            _sourceCache.Connect()
                .Filter(combinedFilter)
                .ObserveOn(RxApp.MainThreadScheduler)  // Update UI in the main stream
                .Bind(out _items)
                .Subscribe();
        }


        // == Virtual Methods with Default Realization ==
        protected virtual bool GetPermission(Permission permission)
        {
            if (_service == null)
                return true;

            return _service.HasPermission(permission);
        }

        protected virtual string GetSearchableText(T item)
        {
            return item?.ToString() ?? string.Empty;
        }

        protected virtual IObservable<Func<T, bool>> GetAdditionalFilters()
        {
            return Observable.Return<Func<T, bool>>(_ => true);
        }

        protected virtual Func<T, bool> CreateSearchFilter(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _ => true;

            var lowerSearch = searchText.ToLower();
            return item => GetSearchableText(item).ToLower().Contains(lowerSearch);
        }

        protected virtual bool CanDeleteOptions()
        {
            return true;
        }

        // To download additional data when opening the panel
        protected virtual void OnViewPanelOpened(T item) { }

        // To roll back changes
        protected virtual void OnCancelEdit() { }

        protected virtual Task OnAddAsync()
        {
            SelectedItem = default;

            CurrentMode = PanelMode.Add;
            IsDetailPanelOpen = true;

            return Task.CompletedTask;
        }


        // == Abstraction Methods ==
        protected abstract Task LoadDataAsync();
        protected abstract Task OnSaveAsync();
        protected abstract Task OnDeleteAsync();


        // == Helper Methods ==
        protected void AddOrUpdateItem(T item) => _sourceCache.AddOrUpdate(item);
        protected void RemoveItem(T item) => _sourceCache.Remove(item);
        protected void ClearItems() => _sourceCache.Clear();


        // == Other Methods ==
        // Open the view panel (double-top)
        private void OpenViewPanel(T item)
        {
            if (item == null) return;
            SelectedItem = item;

            CurrentMode = PanelMode.View;
            IsDetailPanelOpen = true;

            OnViewPanelOpened(item);
        }

        // Changing the view mode to edit mode
        private void EnterEditMode()
        {
            CurrentMode = PanelMode.Edit;
        }

        // Close panel
        private void CloseOrCancelEdit()
        {
            if (CurrentMode == PanelMode.Edit)
            {
                OnCancelEdit();
                CurrentMode = PanelMode.View;
            }
            else
            {
                IsDetailPanelOpen = false;
            }
        }

        // Save Changes in the panel
        private async Task SaveChangesAsync()
        {
            await OnSaveAsync();
            CurrentMode = PanelMode.View;
        }

        // Delete in the panel
        private async Task DeleteFromPanelAsync()
        {
            await OnDeleteAsync();
            IsDetailPanelOpen = false;
        }
    }
}
