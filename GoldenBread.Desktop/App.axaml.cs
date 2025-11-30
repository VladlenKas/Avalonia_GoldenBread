using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels;
using GoldenBread.Desktop.ViewModels.Pages;
using GoldenBread.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GoldenBread.Desktop
{
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider; // DI-container

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
                desktop.MainWindow = new LoginView
                {
                    DataContext = loginViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
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
            services.AddSingleton<UserService>();

            // ViewModels Windows
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<MenuViewModel>();

            services.AddSingleton<UsersPageViewModel>();
            services.AddSingleton<EmployeesPageViewModel>();

            services.AddSingleton<LoginView>();
            services.AddSingleton<MenuView>();
        }
    }
}