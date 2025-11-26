using Avalonia.Controls;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Managers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels.Base;
using GoldenBread.Desktop.ViewModels.Controls;
using GoldenBread.Desktop.ViewModels.Pages;
using GoldenBread.Desktop.Views;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;

namespace GoldenBread.Desktop.ViewModels
{
    public class MenuViewModel : ValidatableViewModelBase
    {
        // == Props ==
        public SidebarViewModel Sidebar { get; }
        public TopbarViewModel Topbar { get; }
        public NavigationManager Navigation { get; }

        // Проброс CurrentPage из NavigationManager
        public object? CurrentPage => Navigation.CurrentPage;

        // == For View ==
        /*public MenuViewModel()
        {
            Sidebar = new SidebarViewModel();
            Topbar = new TopbarViewModel();
        }*/


        // == For Builder ==
        public MenuViewModel(SidebarViewModel sidebar, TopbarViewModel topbar, NavigationManager navigationManager)
        {
            Navigation = navigationManager;
            Sidebar = sidebar;
            Topbar = topbar;

            Navigation.WhenAnyValue(x => x.CurrentPage)
            .Subscribe(page =>
            {
                this.RaisePropertyChanged(nameof(CurrentPage));
            });
        }
    }
}
