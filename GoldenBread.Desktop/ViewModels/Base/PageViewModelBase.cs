using Avalonia.Controls.Documents;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.Base
{
    public abstract class PageViewModelBase<T> : ReactiveValidationObject where T : class
    {
        // == Fields ==
        public readonly SourceCache<T, int> _sourceCache;
        private readonly ReadOnlyObservableCollection<T> _sourceCacheSortered;


        // == Props ==
        [Reactive] public string SearchText { get; set; }
        [Reactive] public T SelectedItem { get; set; }
        [Reactive] public bool IsFilterMenuOpen { get; set; }
        [Reactive] public bool IsLoading { get; set; }


        // == Props for Sorting ==
        [Reactive] public ObservableCollection<string> SortOptions { get; set; }
        [Reactive] public string SelectedSortOption { get; set; }

        public ReadOnlyObservableCollection<T> Items => _sourceCacheSortered;


        // == Commands ==
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
        public ReactiveCommand<Unit, bool> ToggleFilterCommand { get; }


        // == Designer ==
        protected PageViewModelBase(Func<T, int> keySelector)
        {
            _sourceCache = new SourceCache<T, int>(keySelector);

            // Initialization of sorting options
            SortOptions = new ObservableCollection<string>(GetSortOptions());
            SelectedSortOption = SortOptions.FirstOrDefault() ?? "По умолчанию";

            // 1. Load Filters

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

            // Sorting 
            var sortComparer = GetSortComparer();

            // Reactive flow with auto async
            _sourceCache.Connect()
                .Filter(combinedFilter)
                .Sort(sortComparer)
                .ObserveOn(RxApp.MainThreadScheduler)  // Обновления UI в главном потоке
                .Bind(out _sourceCacheSortered)
                .Subscribe();

            // 2. Load Commands

            AddCommand = ReactiveCommand.Create(OnAdd);

            var canDelete = this.WhenAnyValue(x => x.SelectedItem)
                .Select(item => item != null);
            DeleteCommand = ReactiveCommand.Create(OnDelete, canDelete);

            RefreshCommand = ReactiveCommand.CreateFromTask(
                async () =>
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

            ToggleFilterCommand = ReactiveCommand.Create(() =>
                IsFilterMenuOpen = !IsFilterMenuOpen);

            /*Observable.StartAsync(async () =>
            {
                await RefreshCommand.Execute();
            }).Subscribe();*/
        }


        // == Virtual Methods with Default Realization ==

        // List of sorting options
        protected virtual IEnumerable<string> GetSortOptions()
        {
            return new[]
            {
                "По умолчанию",
                "По алфавиту (А-Я)",
                "По алфавиту (Я-А)"
            };
        }

        // Sorting logic for the selected option
        protected virtual IComparer<T> GetSortComparerForOption(string option)
        {
            return option switch
            {
                "По алфавиту (А-Я)" => Comparer<T>.Create((x, y) =>
                    string.Compare(GetSearchableText(x), GetSearchableText(y),
                        StringComparison.OrdinalIgnoreCase)),

                "По алфавиту (Я-А)" => Comparer<T>.Create((x, y) =>
                    string.Compare(GetSearchableText(y), GetSearchableText(x),
                        StringComparison.OrdinalIgnoreCase)),

                _ => GetDefaultSortComparer()
            };
        }

        // Default sorting
        protected virtual IComparer<T> GetDefaultSortComparer()
        {
            return Comparer<T>.Create((x, y) =>
                string.Compare(GetSearchableText(x), GetSearchableText(y),
                    StringComparison.OrdinalIgnoreCase));
        }

        protected virtual string GetSearchableText(T item)
        {
            return item?.ToString() ?? string.Empty;
        }

        protected virtual IComparer<T> GetSortComparer()
        {
            return Comparer<T>.Create((x, y) =>
                string.Compare(GetSearchableText(x), GetSearchableText(y),
                    StringComparison.OrdinalIgnoreCase));
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


        // == Abstraction Methods ==
        protected abstract Task LoadDataAsync();
        protected abstract void OnAdd();
        protected abstract void OnDelete();


        // == Helper Methods
        protected void AddOrUpdateItem(T item) => _sourceCache.AddOrUpdate(item);
        protected void RemoveItem(T item) => _sourceCache.Remove(item);
        protected void ClearItems() => _sourceCache.Clear();
        protected async Task AddOrUpdateItemsAsync(IEnumerable<T> items)
        {
            await Observable.Start(() =>
            {
                foreach (var item in items)
                    _sourceCache.AddOrUpdate(item);
            }, RxApp.TaskpoolScheduler);
        }
    }
}
