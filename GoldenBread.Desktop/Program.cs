using Avalonia;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Managers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.Settings;
using GoldenBread.Desktop.ViewModels;
using GoldenBread.Desktop.ViewModels.Base;
using GoldenBread.Desktop.ViewModels.Controls;
using GoldenBread.Desktop.ViewModels.Pages;
using GoldenBread.Desktop.Views.Controls;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive;
using System.Threading.Tasks;

namespace GoldenBread.Desktop
{
    internal sealed class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            // Global Validator ReactiveUI
            AppDomain.CurrentDomain.UnhandledException += async (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                await ShowErrorAsync("Критическая ошибка", ex?.Message ?? "Неизвестная ошибка", ex?.StackTrace);
            };

            // Validator for tasks
            TaskScheduler.UnobservedTaskException += async (sender, e) =>
            {
                await ShowErrorAsync("Ошибка в задаче", e.Exception.Message, e.Exception.StackTrace);
                e.SetObserved(); // Помечаем как обработанное
            };

            // Validator for reactiveUI
            RxApp.DefaultExceptionHandler = Observer.Create<Exception>(async ex =>
            {
                await ShowErrorAsync("Ошибка ReactiveUI", ex.Message, ex.StackTrace);
            });

            // DI 
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        // Registration of Services
        private static void ConfigureServices(IServiceCollection services)
        {
            // Settings
            var apiSettings = new ApiSettings
            {
                BaseUrl = "https://localhost:7153/",
                TimeoutSeconds = 30
            };
            services.AddSingleton(apiSettings);

            // HttpClient 
            services.AddSingleton<HttpClient>(sp =>
            {
                var settings = sp.GetRequiredService<ApiSettings>();
                var client = new HttpClient
                {
                    BaseAddress = new Uri(settings.BaseUrl),
                    Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds)
                };
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                return client;
            });

            // Services & Managers
            services.AddSingleton<ApiClient>();
            services.AddSingleton<AuthorizationService>();
            services.AddSingleton<ViewService>();
            services.AddSingleton<UserService>();

            services.AddSingleton<NavigationManager>();

            // ViewModels Controls
            services.AddTransient<TopbarViewModel>();
            services.AddSingleton<SidebarViewModel>();

            // ViewModels Pages
            services.AddSingleton<IngredientsViewModel>();
            services.AddSingleton<ProductsViewModel>();
            services.AddSingleton<WarehouseViewModel>();
            services.AddSingleton<EmployeesViewModel>();
            services.AddSingleton<UsersViewModel>();
            services.AddSingleton<CompaniesViewModel>();

            // ViewModels Windows
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MenuViewModel>();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            IconProvider.Current.Register<MaterialDesignIconProvider>();

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .WithInterFont()
                .LogToTrace();
        }

        // Hepler for show errors
        private static async Task ShowErrorAsync(string title, string message, string? stackTrace)
        {
            var fullMessage = $"{message}\n\nStackTrace:\n{stackTrace ?? "Нет информации"}";

            Console.WriteLine($"[{title}] {fullMessage}");

            try
            {
                await MessageBoxManager.GetMessageBoxStandard(
                    title,
                    fullMessage,
                    ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error
                ).ShowAsync();
            }
            catch
            {
                // Если MessageBox не может быть показан, просто логируем
                Console.WriteLine($"Не удалось показать MessageBox: {fullMessage}");
            }
        }
    }
}
