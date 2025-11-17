using Avalonia.Controls;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.Views;
using GoldenBread.Shared.Entities;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;  
using ReactiveUI.Validation.Extensions;
using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.Dialogs
{
    public partial class LoginDialogViewModel : ReactiveBaseViewModel
    {
        // == Filds ==
        private readonly LoginService _loginService;

        // == Props ==
        [Reactive]
        public string Email { get; set; } = string.Empty;

        [Reactive]
        public string Password { get; set; } = string.Empty;

        [Reactive]
        public bool IsDirty { get; set; } = false;

        // == Commands ==
        public ReactiveCommand<Window, Unit> LoginUserCommand { get; }

        // == Methods ==
        public LoginDialogViewModel()
        {
            _loginService = new LoginService();

            this.AddRequiredFieldValidation(x => x.Email, x => x.IsDirty);
            this.AddRequiredFieldValidation(x => x.Password, x => x.IsDirty);

            LoginUserCommand = ReactiveCommand.CreateFromTask<Window>(
                ExecuteLoginAsync,
                this.IsValid());
        }

        private async Task ExecuteLoginAsync(Window window)
        {
            IsDirty = true;
            if (!ValidationContext.GetIsValid()) 
                return;

            var result = await _loginService.LoginAsync(Email, Password);
            if (result.IsSuccess)
            {
                await MessageBoxHelper.ShowOkMessageBox(result.Message);

                var user = LoginService.CurrentUser;
                switch (user?.Role)
                {
                    case UserRole.Admin:
                        new AdminWindow().Show();
                        break;

                    case UserRole.ManagerProduction:
                        new ManagerWindow().Show(); 
                        break;

                    case null:
                        await MessageBoxHelper.ShowOkMessageBox(MessageHelper.UnknownStatus);
                        break;
                }
                window.Close();
            }
            else 
                await MessageBoxHelper.ShowErrorMessageBox(result.Message);
        }
    }
}
