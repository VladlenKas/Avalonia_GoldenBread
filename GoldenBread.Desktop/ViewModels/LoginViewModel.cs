using Avalonia.Controls;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.ViewModels.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System.Reactive;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels
{
    public partial class LoginViewModel : ValidatableViewModelBase
    {
        // ==== Filds ====
        private readonly AuthorizationService _authService;
        private readonly ViewService _viewService;


        // ==== Props ====
        [Reactive] public string Email { get; set; } = string.Empty;
        [Reactive] public string Password { get; set; } = string.Empty;


        // ==== Commands ====
        public ReactiveCommand<Window, Unit> LoginUserCommand { get; }


        // ==== For View ====
        public LoginViewModel()
        {
            this.NotEmpty(this, x => x.Email);
            this.NotEmpty(this, x => x.Password);

            LoginUserCommand = ReactiveCommand.CreateFromTask<Window>(async window =>
            {
                this.ActivateValidation();

                if (!this.ValidationContext.GetIsValid())
                    return;

                await ExecuteLoginAsync(window);
            });
        }


        // ==== For Builder ====
        public LoginViewModel(AuthorizationService authService, ViewService viewService)
        {
            _authService = authService;
            _viewService = viewService;

            NotEmpty(this, x => x.Email);
            NotEmpty(this, x => x.Password);

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
                _viewService.ShowWindow<MenuViewModel>();
                window.Close();
            }
            else 
                await MessageBoxHelper.ShowErrorMessageBox(result.Message);
        }
    }
}
