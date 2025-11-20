using Avalonia;
using Avalonia.Controls;
using GoldenBread.Desktop.ViewModels;
using GoldenBread.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Validation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services
{
    public class ViewService 
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ShowWindow<TViewModel>() where TViewModel : ReactiveValidationObject
        {
            // Get ViewModel from DI
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            // Find view model type for window
            Window window = viewModel switch
            { 
                LoginViewModel vm => new LoginView { DataContext = vm },
                MenuViewModel vm => new MenuView { DataContext = vm },
                _ => throw new InvalidOperationException($"Unknown ViewModel type: {typeof(TViewModel).Name}")
            };

            // Open window
            window.Show();
        }
    }
}
