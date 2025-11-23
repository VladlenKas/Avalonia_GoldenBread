using Avalonia.Controls;
using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels.Pages;
using GoldenBread.Desktop.Views;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
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

namespace GoldenBread.Desktop.ViewModels.Controls
{
    public class TopbarViewModel : ReactiveValidationObject
    {
        // == Filds ==
        private readonly AuthorizationService _authService;
        private readonly SidebarViewModel _sidebar;
        private readonly IServiceProvider _serviceProvider;
        private object? _currentPage;


        // == Props ==
        public ObservableCollection<TopbarItem> TopbarItems { get; }
        public string UserFullname { get; set; }
        public string UserRole { get; set; }
        public object? CurrentPage
        {
            get => _currentPage;
            private set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }


        // == Commands ==
        public ReactiveCommand<TopbarItem, Unit> SelectPageCommand { get; }


        // == For Disigner ==
        public TopbarViewModel()
        {
            TopbarItems = new ObservableCollection<TopbarItem>()
            {
                new TopbarItem("Склад", new IngredientsViewModel()),
                new TopbarItem("Продукты", new IngredientsViewModel()),
                new TopbarItem("Ингредиенты", new IngredientsViewModel()),
            };

            UserFullname = "Касимов Владлен";
            UserRole = "Администратор";
        }


        // == For Builder ==
        public TopbarViewModel(AuthorizationService authService, 
            SidebarViewModel sidebar,
            IServiceProvider serviceProvider)
        {
            _authService = authService;
            _sidebar = sidebar;
            _serviceProvider = serviceProvider;

            UserFullname = _authService.CurrentUser.Fullname;
            UserRole = _authService.CurrentUser.Role.Value.Humanize();

            TopbarItems = new ObservableCollection<TopbarItem>();

            SelectPageCommand = ReactiveCommand.Create<TopbarItem>(item =>
            {
                // Removing the selection from all elements
                foreach (var topbarItem in TopbarItems)
                {
                    topbarItem.IsSelected = false;
                }

                // Setting the selection to the selected element
                item.IsSelected = true;
                CurrentPage = item.ViewModel;
            });


            // Subscribing to the section change
            _sidebar.WhenAnyValue(x => x.SelectedSection)
                .Where(section => section != null)
                .Subscribe(section => LoadPagesForSection(section!.Section));
        }


        // == Methods ==
        private void LoadPagesForSection(SectionType section)
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

                SectionType.Staff => new[]
                {
                    new TopbarItem("Сотрудники", _serviceProvider.GetRequiredService<EmployeesViewModel>()),
                    new TopbarItem("Пользователи", _serviceProvider.GetRequiredService<UsersViewModel>()),
                    new TopbarItem("Компании", _serviceProvider.GetRequiredService<CompaniesViewModel>())
                },
/*
                SectionType.Production => new[]
                {
                    new TopbarItem("Заказы", new OrdersViewModel()),
                    new TopbarItem("График", new ScheduleViewModel())
                },

                SectionType.Analytics => new[]
                {
                    new TopbarItem("Отчёты", new ReportsViewModel()),
                    new TopbarItem("Графики", new ChartsViewModel())
                },*/

                _ => Array.Empty<TopbarItem>()
            };

            foreach (var page in pages)
            {
                TopbarItems.Add(page);
            }

            // Auto select first page
            var firstPage = TopbarItems.FirstOrDefault();
            if (firstPage != null)
            {
                firstPage.IsSelected = true;
                CurrentPage = firstPage.ViewModel;
            }
        }
    }

    public class TopbarItem : ReactiveValidationObject
    {
        private bool _isSelected;

        public string Title { get; set; }
        public object ViewModel { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public TopbarItem(string title, object viewModel)
        {
            Title = title;
            ViewModel = viewModel;
        }
    }
}
