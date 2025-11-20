using Avalonia.Controls;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.Views;
using GoldenBread.Shared.Entities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System.Reactive;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels
{
    public partial class LoginViewModel : ReactiveValidationObject
    {
        // == Filds ==
        private readonly AuthorizationService _authService;
        private readonly ViewService _viewService;


        // == Props ==
        [Reactive] public string Email { get; set; } = string.Empty;
        [Reactive] public string Password { get; set; } = string.Empty;
        [Reactive] public bool IsDirty { get; set; } = false;


        // == Commands ==
        public ReactiveCommand<Window, Unit> LoginUserCommand { get; }


        // == For Disigner ==
        public LoginViewModel()
        {
            this.AddRequiredFieldValidation(x => x.Email, x => x.IsDirty);
            this.AddRequiredFieldValidation(x => x.Password, x => x.IsDirty);

            LoginUserCommand = ReactiveCommand.CreateFromTask<Window>(
                ExecuteLoginAsync,
                this.IsValid());
        }


        // == For Builder ==
        public LoginViewModel(AuthorizationService authService, ViewService viewService)
        {
            _authService = authService;
            _viewService = viewService;

            this.AddRequiredFieldValidation(x => x.Email, x => x.IsDirty);
            this.AddRequiredFieldValidation(x => x.Password, x => x.IsDirty);

            LoginUserCommand = ReactiveCommand.CreateFromTask<Window>(
                ExecuteLoginAsync,
                this.IsValid());
        }


        // == Methods ==
        private async Task ExecuteLoginAsync(Window window)
        {
            IsDirty = true;
            if (!ValidationContext.GetIsValid()) 
                return;

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
