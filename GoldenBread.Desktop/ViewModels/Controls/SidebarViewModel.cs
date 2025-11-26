using Avalonia.Controls;
using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Managers;
using GoldenBread.Desktop.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.Controls
{
    public class SidebarViewModel : ReactiveObject
    {
        // ==== Fields ====
        private readonly NavigationManager _navigation;
        private readonly AuthorizationService _authService;
        private readonly ViewService _viewService;


        // ==== Props ====
        [Reactive] public SidebarItem? SelectedSection { get; set; }
        public ObservableCollection<SidebarItem> Sections { get; }
        public ReactiveCommand<SidebarItem, Unit> SelectSectionCommand { get; }
        public ReactiveCommand<Window, Unit> LogoutCommand { get; }


        // ==== Designer ====
        public SidebarViewModel(
            NavigationManager navigation, 
            AuthorizationService authService, 
            ViewService viewService)
        {
            _navigation = navigation;
            _authService = authService;
            _viewService = viewService;

            Sections = new ObservableCollection<SidebarItem>
            {
                new SidebarItem(SectionType.References, "mdi-bookmark-box-outline"),
                new SidebarItem(SectionType.Staff, "mdi-account-group-outline"),
                new SidebarItem(SectionType.Production, "mdi-clipboard-text-clock-outline"),
                new SidebarItem(SectionType.Analytics, "mdi-chart-line")
            };

            SelectSectionCommand = ReactiveCommand.Create<SidebarItem>(section =>
            {
                SelectedSection = section;
            });

            LogoutCommand = ReactiveCommand.CreateFromTask<Window>(ExecuteLogoutAsync);
        }


        // ==== Command ====
        private async Task ExecuteLogoutAsync(Window window)
        {
            var result = await MessageBoxHelper
                .ShowQuestionMessageBox("Вы действительно хотите выйти?");

            if (result)
            {
                _viewService.ShowWindow<LoginViewModel>();
                window.Close();
                _authService.Logout();
            }
        }
    }
}
