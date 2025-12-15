using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using GoldenBread.Desktop.ViewModels.Pages;
using GoldenBread.Desktop.Services.Api;
using GoldenBread.Desktop.Bases;

namespace GoldenBread.Desktop.ViewModels
{
    public class SectionViewModel : ViewModelValidationBase
    {
        public string Name { get; set; } = null!;
        public MaterialIconKind IconKey { get; set; }
        public ObservableCollection<PageInfo> Pages { get; set; } = null!;
    }

    public class PageInfo
    {
        public string Title { get; set; } = null!;
        public Type PageType { get; set; } = null!;
    }

    public class MenuViewModel : ViewModelValidationBase
    {
        private AuthorizationApiService _authService;

        [Reactive] public SectionViewModel SelectedSection { get; set; }
        [Reactive] public PageInfo SelectedPage { get; set; }
        [Reactive] public ViewModelValidationBase CurrentPageContent { get; set; }

        [Reactive] public bool IsPaneOpen { get; set; } = true;
        public ReactiveCommand<Unit, bool> TogglePaneCommand { get; }
        public ReactiveCommand<SectionViewModel, Unit> SelectSectionCommand { get; }
        public ObservableCollection<SectionViewModel> Sections { get; } = new()
        {
            new SectionViewModel
            {
                Name = "Справочники",
                Pages = new()
                {
                    new PageInfo { Title = "Ингредиенты", PageType = typeof(UsersPageViewModel) },
                    new PageInfo { Title = "Продукция", PageType = typeof(EmployeesPageViewModel) },
                    new PageInfo { Title = "Склад", PageType = typeof(EmployeesPageViewModel) }
                },
                IconKey = MaterialIconKind.BookmarkBoxOutline
            },
            new SectionViewModel
            {
                Name = "Персонал",
                Pages = new()
                {
                    new PageInfo { Title = "Пользователи", PageType = typeof(UsersPageViewModel) },
                    new PageInfo { Title = "Компании", PageType = typeof(EmployeesPageViewModel) },
                    new PageInfo { Title = "Сотрудники", PageType = typeof(UsersPageViewModel) }
                },
                IconKey = MaterialIconKind.Analytics
            },
            new SectionViewModel
            {
                Name = "Производство",
                Pages = new()
                {
                    new PageInfo { Title = "Заказы", PageType = typeof(UsersPageViewModel) },
                    new PageInfo { Title = "График работ", PageType = typeof(EmployeesPageViewModel) },
                },
                IconKey = MaterialIconKind.Analytics
            },
            new SectionViewModel
            {
                Name = "Аналитика",
                Pages = new()
                {
                    new PageInfo { Title = "Статистика", PageType = typeof(UsersPageViewModel) },
                    new PageInfo { Title = "Производительность", PageType = typeof(EmployeesPageViewModel) },
                    new PageInfo { Title = "Контрольные значения", PageType = typeof(UsersPageViewModel) }
                },
                IconKey = MaterialIconKind.Analytics
            },
        };

        public MenuViewModel(IServiceProvider serviceProvider)
        {
            TogglePaneCommand = ReactiveCommand.Create(() => IsPaneOpen = !IsPaneOpen);
            SelectSectionCommand = ReactiveCommand.Create<SectionViewModel>(section =>
            {
                SelectedSection = section;
            });

            // Автоматически выбираем первую страницу при смене раздела
            this.WhenAnyValue(x => x.SelectedSection)
                .Where(section => section?.Pages.Count > 0)
                .Subscribe(section => SelectedPage = section.Pages[0]);

            // Автоматически создаем ViewModel при выборе страницы
            this.WhenAnyValue(x => x.SelectedPage)
                .WhereNotNull()
                .Subscribe(page =>
                {
                    var vm = serviceProvider.GetRequiredService(page.PageType) as ViewModelValidationBase;
                    CurrentPageContent = vm;
                });
        }
        // == For Builder ==
        public MenuViewModel(IServiceProvider serviceProvider, AuthorizationApiService authService)
        {
            _authService = authService;

            TogglePaneCommand = ReactiveCommand.Create(() => IsPaneOpen = !IsPaneOpen);
            SelectSectionCommand = ReactiveCommand.Create<SectionViewModel>(section =>
            {
                SelectedSection = section;
            });

            // Автоматически выбираем первую страницу при смене раздела
            this.WhenAnyValue(x => x.SelectedSection)
                .Where(section => section?.Pages.Count > 0)
                .Subscribe(section => SelectedPage = section.Pages[0]);

            // Автоматически создаем ViewModel при выборе страницы
            this.WhenAnyValue(x => x.SelectedPage)
                .WhereNotNull()
                .Subscribe(page =>
                {
                    var vm = serviceProvider.GetRequiredService(page.PageType) as ViewModelValidationBase;
                    CurrentPageContent = vm;
                });
        }
    }
}
