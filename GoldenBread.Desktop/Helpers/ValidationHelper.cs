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

    public static class ValidationRules
    {
        // Required Field
        public static void AddRequiredFieldValidation<TViewModel, TProp>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, TProp>> property,
        Expression<Func<TViewModel, bool>> isDirtyProperty,
        string message = ValidationMessages.Required)
        where TViewModel : IReactiveObject, IValidatableViewModel
        {
            // Проверка на пустоту
            viewModel.ValidationRule(
                property,
                viewModel.WhenAnyValue(
                    property,
                    isDirtyProperty,
                    (value, isDirty) => !isDirty || !string.IsNullOrWhiteSpace(value?.ToString())),
                message);
        }

        // Email Field
        public static void AddEmailValidation<TViewModel>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, string>> emailProperty,
        Expression<Func<TViewModel, bool>> isDirtyProperty)
        where TViewModel : IReactiveObject, IValidatableViewModel  
        {
            // Проверка на пустоту
            viewModel.ValidationRule(
                emailProperty,
                viewModel.WhenAnyValue(
                    emailProperty,
                    isDirtyProperty,
                    (email, isDirty) => !isDirty || !string.IsNullOrWhiteSpace(email)),
                ValidationMessages.Required);

            // Проверка формата
            viewModel.ValidationRule(
                emailProperty,
                viewModel.WhenAnyValue(
                    emailProperty,
                    email => string.IsNullOrWhiteSpace(email) || ValidationRegexes.EmailRegex.IsMatch(email)),
                ValidationMessages.InvalidEmailFormat);
        }

        // Password Field
        public static void AddPasswordValidation<TViewModel>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, string>> passwordProperty,
        Expression<Func<TViewModel, bool>> isDirtyProperty,
        int minLength = 6)
        where TViewModel : IReactiveObject, IValidatableViewModel 
        {
            // Проверка на пустоту
            viewModel.ValidationRule(
                passwordProperty,
                viewModel.WhenAnyValue(
                    passwordProperty,
                    isDirtyProperty,
                    (password, isDirty) => !isDirty || !string.IsNullOrWhiteSpace(password)),
                ValidationMessages.Required);

            // Проверка мин. длины
            viewModel.ValidationRule(
                passwordProperty,
                viewModel.WhenAnyValue(
                    passwordProperty,
                    password => string.IsNullOrWhiteSpace(password) || password.Length >= minLength),
                ValidationMessages.PasswordTooShort);
        }
    }
}
