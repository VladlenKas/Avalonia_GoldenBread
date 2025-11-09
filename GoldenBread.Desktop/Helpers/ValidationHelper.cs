using FluentValidation;
using GoldenBread.Shared.Enums.User;
using GoldenBread.Desktop.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using ReactiveValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Helpers
{
    internal static class ValidationHelper
    {
        // Добавляет правило валидации на непустое поле с поддержкой задержки срабатывания
        public static void EmailValidation<TViewModel>(
        this TViewModel viewModel, 
        Expression<Func<TViewModel, string>> emailProperty)
        where TViewModel : ReactiveValidationObject
        {
            viewModel.ValidationRule(
                emailProperty,  
                email => !string.IsNullOrWhiteSpace(email),
                "Email не может быть пустым");

            viewModel.ValidationRule(
                emailProperty,
                email => !IsValidEmail(email),
                "Электронная почта не соответсвует формату");
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public class LoginUserRequestValidator : AbstractValidator<LoginUser>
        {
            public LoginUserRequestValidator()
            {
                RuleFor(x => x.Login)
                    .NotEmpty().WithMessage("Логин обязателен")
                    .MinimumLength(3).WithMessage("Минимум 3 символа")
                    .MaximumLength(50).WithMessage("Максимум 50 символов");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Пароль обязателен")
                    .MinimumLength(6).WithMessage("Минимум 6 символов");
            }
        }
    }
}
