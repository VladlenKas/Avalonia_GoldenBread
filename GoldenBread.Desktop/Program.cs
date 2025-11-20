using Avalonia;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels;
using GoldenBread.Desktop.ViewModels.Controls;
using GoldenBread.Desktop.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System;
using System.Reactive;

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
            RxApp.DefaultExceptionHandler = Observer.Create<Exception>(async ex =>
            {
                await MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка", 
                    $"ReactiveUI Error: {ex.Message}\n\nStackTrace: {ex.StackTrace}", 
                    ButtonEnum.Ok, 
                    MsBox.Avalonia.Enums.Icon.Error).ShowAsync();

                Console.WriteLine($"ReactiveUI Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
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
            // Services
            services.AddSingleton<AuthorizationService>();
            services.AddSingleton<ViewService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MenuViewModel>();

            services.AddTransient<SidebarViewModel>();
            services.AddTransient<TopbarViewModel>();

            services.AddTransient<IngredientsViewModel>();
            services.AddTransient<ProductsViewModel>();
            services.AddTransient<WarehouseViewModel>();
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
    }
}
