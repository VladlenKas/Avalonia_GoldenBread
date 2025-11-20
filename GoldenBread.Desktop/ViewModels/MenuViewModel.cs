using Avalonia.Controls;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels.Controls;
using GoldenBread.Desktop.ViewModels.Pages;
using GoldenBread.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels
{
    public class MenuViewModel : ReactiveValidationObject
    {
        // == Filds ==
        private readonly AuthorizationService _authService;
        private readonly ViewService _viewService;


        // == Props ==
        public SidebarViewModel Sidebar { get; }
        public TopbarViewModel Topbar { get; }


        // == For Disigner ==
        public MenuViewModel()
        {
            Sidebar = new SidebarViewModel();
            Topbar = new TopbarViewModel();
        }


        // == For Builder ==
        public MenuViewModel(AuthorizationService authService, 
            ViewService viewService)
        {
            _authService = authService;
            _viewService = viewService;

            Sidebar = new SidebarViewModel(authService, viewService);
            Topbar = new TopbarViewModel(authService, Sidebar);
        }
    }
}
