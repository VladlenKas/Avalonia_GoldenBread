using GoldenBread.Desktop.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using System.Reactive;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        // Поля
        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _isDirty = false;
        private readonly UserService _userService;

        // Конструктор
        public MainWindowViewModel()
        {
            _userService = new UserService();

            this.ValidationRule(
                x => x.Email,
                this.WhenAnyValue(x => x.Email, x => x.IsDirty,
                    (email, isDirty) => !isDirty || !string.IsNullOrWhiteSpace(email)),
                "Это обязательное поле!");

            this.ValidationRule(
                x => x.Password,
                this.WhenAnyValue(x => x.Password, x => x.IsDirty,
                    (pass, isDirty) => !isDirty || !string.IsNullOrWhiteSpace(pass)),
                "Это обязательное поле!");

            LoginUserCommand = ReactiveCommand.CreateFromTask(
                ExecuteLogin,
                this.IsValid());
        }

        // Свойства
        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public bool IsDirty
        {
            get => _isDirty;
            set => this.RaiseAndSetIfChanged(ref _isDirty, value);
        }

        // Комманды
        public ReactiveCommand<Unit, Unit> LoginUserCommand { get; }

        // Методы
        private async Task ExecuteLogin()
        {
            IsDirty = true;

            if (!ValidationContext.GetIsValid())
                return;

            await Task.Delay(500);

            var result = await _userService.LoginAsync(Email, Password);

            if (result.IsSuccess)
            {
                await MessageBoxManager.GetMessageBoxStandard(
                    "Успех!",
                    result.Message,
                    ButtonEnum.Ok,
                    Icon.Success).ShowAsync();
            }
            else
            {
                // Ошибка: error содержит сообщение
                await MessageBoxManager.GetMessageBoxStandard(
                    "Ошибка",
                    result.Message,
                    ButtonEnum.Ok,
                    Icon.Error).ShowAsync();
            }
        }
    }
}
