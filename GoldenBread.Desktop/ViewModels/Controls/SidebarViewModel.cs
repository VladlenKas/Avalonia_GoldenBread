using Avalonia.Controls;
using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels.Pages;
using GoldenBread.Desktop.Views;
using GoldenBread.Shared.Entities;
using Humanizer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Projektanker.Icons.Avalonia;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace GoldenBread.Desktop.ViewModels.Controls
{
    public class SidebarViewModel : ReactiveValidationObject
    {
        // == Filds ==
        private readonly AuthorizationService _authService;
        private readonly ViewService _viewService;
        private SidebarItem? _selectedSection;


        // == Props ==
        public ObservableCollection<SidebarItem> SidebarItems { get; }
        public SidebarItem? SelectedSection
        {
            get => _selectedSection;
            set
            {
                // Removing the selection from the old element
                if (_selectedSection != null)
                    _selectedSection.IsSelected = false;

                this.RaiseAndSetIfChanged(ref _selectedSection, value);

                // Setting the selection to a new element
                if (_selectedSection != null)
                    _selectedSection.IsSelected = true;
            }
        }


        // == Commands ==
        public ReactiveCommand<SidebarItem, Unit> SelectSectionCommand { get; }
        public ReactiveCommand<Window, Unit> LogoutCommand { get; }


        // == For View ==
        public SidebarViewModel()
        {
            SidebarItems = new ObservableCollection<SidebarItem>()
            {
                new SidebarItem(SectionType.References, "mdi-bookmark-box-outline"),
                new SidebarItem(SectionType.Staff, "mdi-account-group-outline"),
                new SidebarItem(SectionType.Production, "mdi-clipboard-text-clock-outline"),
                new SidebarItem(SectionType.Analytics, "mdi-chart-line")
            };
        }


        // == For Builder ==
        public SidebarViewModel(AuthorizationService authService, ViewService viewService)
        {
            _authService = authService;
            _viewService = viewService;

            SidebarItems = Initialize(_authService.CurrentUser);

            SelectSectionCommand = ReactiveCommand.Create<SidebarItem>(section =>
            {
                SelectedSection = section;
            });
            LogoutCommand = ReactiveCommand.CreateFromTask<Window>(ExecuteLogoutAsync);
        }


        // == Methods ==
        private ObservableCollection<SidebarItem> Initialize(User currentUser)
        {
            var items = new ObservableCollection<SidebarItem>();

            // Pages for anyone
            items.Add(new SidebarItem(SectionType.References, "mdi-bookmark-box-outline"));
            items.Add(new SidebarItem(SectionType.Staff, "mdi-account-group-outline"));
            items.Add(new SidebarItem(SectionType.Production, "mdi-clipboard-text-clock-outline"));
            items.Add(new SidebarItem(SectionType.Analytics, "mdi-chart-line"));

            return items;
        }

        private async Task ExecuteLogoutAsync(Window window)
        {
            var result = await MessageBoxHelper.ShowQuestionMessageBox("Вы действительно хотите выйти?");

            if (result)
            {
                _viewService.ShowWindow<LoginViewModel>();
                window.Close();
                _authService.Logout();
            }
        }
    }

    public class SidebarItem : ReactiveValidationObject
    {
        private bool _isSelected;

        public string Title { get; set; }
        public SectionType Section { get; set; }
        public object IconTag { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public SidebarItem(SectionType section, object iconTag)
        {
            Section = section;
            Title = section.Humanize();
            IconTag = iconTag;
        }
    }
}
