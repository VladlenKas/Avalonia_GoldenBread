using Avalonia.Controls;
using Avalonia.Data;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Desktop.Services;
using GoldenBread.Desktop.Views;
using GoldenBread.Desktop.Views.Dialogs;
using GoldenBread.Shared.Entities;
using Humanizer;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels
{
    public partial class AdminWindowViewModel : ReactiveObject
    {
        // == Filds ==
        private readonly LoginService _loginService;

        // == Props ==

        [Reactive]
        public string UserFullname { get; set; }

        [Reactive]
        public string UserRole { get; set; }

        // == Commands ==
        public ReactiveCommand<Window, Unit> LogoutCommand { get; }

        // == Methods ==
        public AdminWindowViewModel()
        {
            _loginService = new LoginService();
            LogoutCommand = ReactiveCommand.CreateFromTask<Window>(ExecuteLogoutAsync);

            UserFullname = LoginService.CurrentUser?.Fullname ?? "Не указано";
            UserRole = LoginService.CurrentUser?.Role.Value.Humanize() ?? "Не указано";
        }

        private async Task ExecuteLogoutAsync(Window window)
        {
            // Показываем MessageBox с вопросом
            var result = await MessageBoxHelper.ShowQuestionMessageBox("Вы действительно хотите выйти?");

            if (result)
            {
                _loginService.Logout();
                new LoginDialog().Show();
                window.Close();
            }
        }
    }
}
