using FluentValidation;
using GoldenBread.Desktop.ViewModels;
using GoldenBread.Domain.Requests;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using ReactiveValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Helpers
{
    internal static class ValidationMessages
    {
        public const string Required = "Это обязательное поле!";
        public const string InvalidEmailFormat = "Неверный формат email!";
        public const string EmailTooLong = "Email не должен превышать 254 символа!";
        public const string PasswordTooShort = "Пароль должен быть не менее 6 символов!";
    }

    internal static class ValidationRegexes
    { 
        public static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Regex PhoneRegex = new Regex(
            @"^(\+7|8)?[\s\-]?\(?\d{3}\)?[\s\-]?\d{3}[\s\-]?\d{2}[\s\-]?\d{2}$",
            RegexOptions.Compiled);

        public static readonly Regex NameRegex = new Regex(
            @"^[а-яА-ЯёЁ\s\-]+$",
            RegexOptions.Compiled);
    }
}
