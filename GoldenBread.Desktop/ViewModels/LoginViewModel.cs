using Avalonia.Controls;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels.Base;
using GoldenBread.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Splat;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        // ==== Filds ====
        private readonly AuthorizationService _authService;
        private readonly IServiceProvider _serviceProvider;


        // ==== Props ====
        [Reactive] public string Email { get; set; } = string.Empty;
        [Reactive] public string Password { get; set; } = string.Empty;


        // ==== Commands ====
        public ReactiveCommand<Window, Unit> LoginUserCommand { get; }


        // ==== For Builder ====
        public LoginViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _authService = _serviceProvider.GetRequiredService<AuthorizationService>();

            ValidateRequired(this, x => x.Email);
            ValidateRequired(this, x => x.Password);

            LoginUserCommand = ReactiveCommand.CreateFromTask<Window>(async window =>
            {
                var success = Validate();
                if (!success) return;   

                await ExecuteLoginAsync(window);
            });
        }


        // ==== Methods ====
        private async Task ExecuteLoginAsync(Window window)
        {
            var result = await _authService.LoginAsync(Email, Password);
            if (_authService.IsAuthenticated)
            {
                await MessageBoxHelper.ShowOkMessageBox(result.Message);

                var menu = _serviceProvider.GetRequiredService<MenuView>();
                menu.DataContext = _serviceProvider.GetRequiredService<MenuViewModel>();
                menu.Show();

                window.Close();
            }
            else 
                await MessageBoxHelper.ShowErrorMessageBox(result.Message);
        }
    }
}
