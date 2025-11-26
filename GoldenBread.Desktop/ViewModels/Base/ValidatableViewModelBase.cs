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
    public class ValidatableViewModelBase :  ReactiveValidationObject
    {
        [Reactive] private bool IsDirty { get; set; }

        public void ActivateValidation() => IsDirty = true;
        public void DeactivateValidation() => IsDirty = false;
        public bool Validate()
        {
            ActivateValidation();

            if (!ValidationContext.GetIsValid())
                return false;

            return true;
        }

        // ==== Validation Rules ====
        public ValidationHelper NotEmpty<TViewModel>(
        TViewModel viewModel,
        Expression<Func<TViewModel, string>> property)
        where TViewModel : ValidatableViewModelBase
        {
            return viewModel.ValidationRule(
                property,
                viewModel.WhenAnyValue(
                    property,
                    vm => vm.IsDirty,
                    (value, isDirty) => !isDirty || !string.IsNullOrWhiteSpace(value?.ToString())),
                ValidationMessages.Required);
        }
    }
}
