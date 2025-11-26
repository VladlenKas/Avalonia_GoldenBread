using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Managers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace GoldenBread.Desktop.ViewModels.Controls
{
    public class TopbarViewModel : ReactiveObject
    {
        // ==== Fields ====
        private readonly NavigationManager _navigation;
        private readonly IServiceProvider _serviceProvider;
        private readonly AuthorizationService _authService;


        // ==== Props ====
        public ObservableCollection<TopbarItem> TopbarItems { get; }
        public ReactiveCommand<TopbarItem, Unit> SelectPageCommand { get; }
        public string UserFullname { get; }
        public string UserRole { get; }


        // ==== Designer ====
        public TopbarViewModel(
            NavigationManager navigation,
            IServiceProvider serviceProvider,
            AuthorizationService authService,
            SidebarViewModel sidebar)
        {
            _navigation = navigation;
            _serviceProvider = serviceProvider;
            _authService = authService;

            TopbarItems = new ObservableCollection<TopbarItem>();

            SelectPageCommand = ReactiveCommand.Create<TopbarItem>(page =>
            {
                foreach (var item in TopbarItems)
                    item.IsSelected = false;

                page.IsSelected = true;
                _navigation.NavigateTo(page.ViewModel);
            });

            UserFullname = authService.CurrentUser.Fullname;
            UserRole = authService.CurrentUser.RoleValue;

            sidebar.WhenAnyValue(x => x.SelectedSection)
                .Where(section => section != null)
                .Subscribe(section => LoadSection(section!.Section));
        }


        // ==== Methods ====
        private void LoadSection(SectionType section)
        {
            TopbarItems.Clear();

            var pages = section switch
            {
                SectionType.References => new[]
                {
                    new TopbarItem("Склад", _serviceProvider.GetRequiredService<WarehouseViewModel>()),
                    new TopbarItem("Продукты", _serviceProvider.GetRequiredService<ProductsViewModel>()),
                    new TopbarItem("Ингредиенты", _serviceProvider.GetRequiredService<IngredientsViewModel>())
                },

                SectionType.Staff => GetStaffPages(),

                _ => Array.Empty<TopbarItem>()
            };

            foreach (var page in pages)
                TopbarItems.Add(page);

            var firstPage = TopbarItems.FirstOrDefault();
            if (firstPage != null)
                SelectPageCommand.Execute(firstPage).Subscribe();
        }

        private TopbarItem[] GetStaffPages()
        {
            var pages = new List<TopbarItem>
            {
                new TopbarItem("Сотрудники", _serviceProvider.GetRequiredService<EmployeesViewModel>()),
                new TopbarItem("Компании", _serviceProvider.GetRequiredService<CompaniesViewModel>())
            };

            if (_authService.CurrentRole == Domain.Models.UserRole.Admin)
            {
                pages.Add(new TopbarItem("Пользователи", _serviceProvider.GetRequiredService<UsersViewModel>()));
            }

            return pages.ToArray();
        }
    }
}
