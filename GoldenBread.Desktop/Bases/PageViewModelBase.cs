using DynamicData;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.Services.Crud;
using GoldenBread.Desktop.ViewModels.Controls;
using GoldenBread.Domain.Models;
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

namespace GoldenBread.Desktop.Bases;

public class PageViewModelBase<T> : ViewModelValidationBase, IPageViewModel where T : class, new()
{
    // ==== Fields ====
    private readonly SourceCache<T, int> _itemsCache;
    private readonly ReadOnlyObservableCollection<T> _items;

    // ==== Props ====
    public ReadOnlyObservableCollection<T> Items => _items;
    public DetailsPanelViewModel<T> DetailsPanel { get; }
    [Reactive] public T? SelectedItem { get; set; }
    protected virtual Func<T, int> KeySelector { get; set; }

    // ==== Commands ====
    public ReactiveCommand<Unit, Unit> CreateCommand { get; }

    // ==== Designer ====
    public PageViewModelBase(ICrudService<T> userCrudService, Func<T, int> keySelector)
    {
        DetailsPanel = new DetailsPanelViewModel<T>(userCrudService);
        CreateCommand = ReactiveCommand.Create(() => DetailsPanel.ShowCreate());

        this.WhenAnyValue(x => x.SelectedItem)
            .WhereNotNull()
            .Subscribe(m => DetailsPanel.ShowDetails(m));

        DetailsPanel.WhenAnyValue(x => x.Mode)
            .Where(mode => mode == PanelMode.Hidden)
            .Subscribe(_ => SelectedItem = default);

        KeySelector = keySelector;
        _itemsCache = new SourceCache<T, int>(KeySelector);
        _itemsCache.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

        // Subscribe on crud methods
        DetailsPanel.OnEntitySaved += OnEntitySaved;
        DetailsPanel.OnEntityDeleted += OnEntityDeleted;
    }

    // ==== Crud Methods ====
    private void OnEntitySaved(T savedEntity)
    {
        _itemsCache.AddOrUpdate(savedEntity);
    }

    private void OnEntityDeleted(int entityId)
    {
        _itemsCache.RemoveKey(entityId);
    }

    protected void AddOrUpdateItems(IEnumerable<T> items)
    {
        _itemsCache.Edit(innerCache =>
        {
            foreach (var item in items)
                innerCache.AddOrUpdate(item);
        });
    }

    protected void ClearItems()
    {
        _itemsCache.Clear();
    }

    // ==== Interfaces ====
    object? IPageViewModel.SelectedItem
    {
        get => SelectedItem;
        set => SelectedItem = value as T;
    }
    IDetailsPanelViewModel IPageViewModel.DetailsPanel => (IDetailsPanelViewModel)DetailsPanel;
}
