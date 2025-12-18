using Avalonia.Controls;
using GoldenBread.Desktop.Bases;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services.Api;
using GoldenBread.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels
{
    public partial class LoginViewModel : ViewModelValidationBase
    {
        private readonly AuthorizationApiService _authService;
        private readonly IServiceProvider _serviceProvider;

        [Reactive] public string Email { get; set; } = string.Empty;
        [Reactive] public string Password { get; set; } = string.Empty;

        public ReactiveCommand<Window, Unit>? LoginUserCommand { get; set; }

        public LoginViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _authService = _serviceProvider.GetRequiredService<AuthorizationApiService>();

            CreateValidationRules();
            CreateCommands();
        }

        private void CreateValidationRules()
        {
            ValidateRequired(this, x => x.Email);
            ValidateRequired(this, x => x.Password);
        }

        private void CreateCommands()
        {
            var canLogin = this.WhenAnyValue(x => x.IsDirty)
                .Select(dirty => !dirty)
                .Merge(ValidationContext.Valid);

            LoginUserCommand = ReactiveCommand.CreateFromTask<Window>(
                ExecuteLoginAsync,
                canLogin);
        }

        private async Task ExecuteLoginAsync(Window window)
        {
            if (!Validate())
            {
                return;
            }

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
            {
                await MessageBoxHelper.ShowErrorMessageBox(result.Message);
            }
        }
    }
}
