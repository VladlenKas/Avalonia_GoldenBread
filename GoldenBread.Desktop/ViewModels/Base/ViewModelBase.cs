using GoldenBread.Desktop.Helpers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.Base
{
    public class ViewModelBase :  ReactiveValidationObject
    {
        [Reactive] private bool IsDirty { get; set; }

        public void ActivateValidation() => IsDirty = true;
        public void DeactivateValidation() => IsDirty = false;
        public bool Validate()
        {
            ActivateValidation();

            if (ValidationContext.GetIsValid())
                return true;

            return false;
        }

        // ==== Validation Rules ====
        // Not Empty
        public ValidationHelper ValidateRequired<TViewModel>(
        TViewModel viewModel,
        Expression<Func<TViewModel, string>> property)
        where TViewModel : ViewModelBase
        {
            return viewModel.ValidationRule(
                property,
                viewModel.WhenAnyValue(
                    property,
                    vm => vm.IsDirty,
                    (value, isDirty) => !isDirty || !string.IsNullOrWhiteSpace(value)),
                ValidationMessages.Required);
        }

        // Age
        public ValidationHelper ValidateAge<TViewModel>(
        TViewModel viewModel,
        Expression<Func<TViewModel, string>> property)
        where TViewModel : ViewModelBase
        {
            return viewModel.ValidationRule(
                property,
                viewModel.WhenAnyValue(
                    property,
                    vm => vm.IsDirty,
                    (value, isDirty) =>
                    {
                        if (!isDirty || string.IsNullOrWhiteSpace(value))
                            return true;

                        if (DateTime.TryParseExact(value, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime date))
                        {
                            var age = DateTime.Today.Year - date.Year;
                            if (date > DateTime.Today.AddYears(-age)) age--;
                            return age >= 18 || age <= 90;
                        }
                        return true; 
                    }),
                ValidationMessages.InvalidAge);
        }

        // Date format
        public ValidationHelper ValidateDateFormat<TViewModel>(
        TViewModel viewModel,
        Expression<Func<TViewModel, string>> property)
        where TViewModel : ViewModelBase
        {
            return viewModel.ValidationRule(
                property,
                viewModel.WhenAnyValue(
                    property,
                    vm => vm.IsDirty,
                    (value, isDirty) => !isDirty ||
                        DateTime.TryParseExact(value, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out _)),
                ValidationMessages.InvalidDateFormat);
        }
    }
}
