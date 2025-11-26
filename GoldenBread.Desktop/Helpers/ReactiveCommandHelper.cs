using Avalonia.Controls;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.ViewModels.Base;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Helpers
{
    public static class ReactiveCommandHelper
    {
        /// <summary>
        /// It is used for commands that activate validation after clicking
        /// without parameter
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="vm"></param>
        /// <param name="execute"></param>
        /// <returns></returns>
        public static ReactiveCommand<Unit, Unit> CreateValidatedCommand<TViewModel>(
        this TViewModel vm,
        Func<Task> execute)
        where TViewModel : ValidatableViewModelBase
        {
            return ReactiveCommand.CreateFromTask(async () =>
            {
                vm.ActivateValidation();

                if (!vm.ValidationContext.GetIsValid())
                    return;

                await execute();
            }, vm.IsValid());
        }

        /// <summary>
        /// It is used for commands that activate validation after clicking
        /// with window parameter
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="vm"></param>
        /// <param name="execute"></param>
        /// <returns></returns>
        public static ReactiveCommand<Window, Unit> CreateValidatedCommand<TViewModel>(
        this TViewModel vm,
        Func<Window, Task> execute)
        where TViewModel : ValidatableViewModelBase
        {
            return ReactiveCommand.CreateFromTask<Window>(async window =>
            {
                vm.ActivateValidation();

                if (!vm.ValidationContext.GetIsValid())
                    return;

                await execute(window);
            }, vm.IsValid());
        }
    }
}
