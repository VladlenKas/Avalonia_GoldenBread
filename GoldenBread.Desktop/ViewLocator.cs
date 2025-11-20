using Avalonia.Controls;
using Avalonia.Controls.Templates;
using GoldenBread.Desktop.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;
using System;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GoldenBread.Desktop
{
    public class ViewLocator : IDataTemplate
    {

        public Control? Build(object? param)
        {

            if (param is null)
                return null;

            var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);

            if (type != null)
            {
                // Find View from DI
                var view = Program.ServiceProvider.GetService(type) as Control;

                return view ?? (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ReactiveValidationObject;
        }
    }
}
