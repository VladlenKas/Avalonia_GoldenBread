using Avalonia.Controls.Documents;
using DynamicData;
using GoldenBread.Desktop.Enums;
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
    public abstract class PageViewModelBase<T> : ReactiveValidationObject where T : class
    {
        // == Fields ==
        public readonly SourceCache<T, int> _sourceCache;
        private readonly ReadOnlyObservableCollection<T> _items;
        private readonly AuthorizationService _service;


        // == Props ==
        [Reactive] public string SearchText { get; set; }
        [Reactive] public T SelectedItem { get; set; }
        [Reactive] public bool IsLoading { get; set; }
        [Reactive] public bool IsFilterMenuOpen { get; set; }

        public ReadOnlyObservableCollection<T> Items => _items;


        // == Access rights ==
        public bool CanAdd { get; }
        public bool CanEdit { get; }
        public bool CanDelete { get; }


        // == Commands ==
        public ReactiveCommand<Unit, Unit>? AddCommand { get; }
        public ReactiveCommand<Unit, Unit>? EditCommand { get; }
        public ReactiveCommand<Unit, Unit>? DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Unit, bool> ToggleFilterCommand { get; }


        // == Designer ==
        protected PageViewModelBase(Func<T, int> keySelector, AuthorizationService service)
        {
            _sourceCache = new SourceCache<T, int>(keySelector);
            _service = service;

            // 1. Distribution of rights
            CanAdd = GetPermission(Permission.Add);
            CanEdit = GetPermission(Permission.Edit);
            CanDelete = GetPermission(Permission.Delete);

            // 2. Load Commands
            if (CanAdd)
            {
                AddCommand = ReactiveCommand.CreateFromTask(OnAddAsync);
            }

            if (CanEdit)
            {
                var canEdit = this.WhenAnyValue(x => x.SelectedItem)
                    .Select(item => item != null);
                EditCommand = ReactiveCommand.CreateFromTask(OnEditAsync, canEdit);
            }

            if (CanDelete)  
            {
                var canDelete = this.WhenAnyValue(x => x.SelectedItem)
                    .Select(item => item != null && CanDeleteOptions());
                DeleteCommand = ReactiveCommand.CreateFromTask(OnDeleteAsync, canDelete);
            }

            ToggleFilterCommand = ReactiveCommand.Create(() =>
                IsFilterMenuOpen = !IsFilterMenuOpen);

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


        // == Abstraction Methods ==
        protected abstract Task LoadDataAsync();
        protected abstract Task OnAddAsync();
        protected abstract Task OnEditAsync();
        protected abstract Task OnDeleteAsync();


        // == Helper Methods ==
        protected void AddOrUpdateItem(T item) => _sourceCache.AddOrUpdate(item);
        protected void RemoveItem(T item) => _sourceCache.Remove(item);
        protected void ClearItems() => _sourceCache.Clear();
    }
}
