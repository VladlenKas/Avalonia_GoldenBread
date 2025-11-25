using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Managers
{
    public class NavigationManager : ReactiveObject
    {
        [Reactive] public object? CurrentPage { get; set; }

        public void NavigateTo(object viewModel)
        {
            CurrentPage = viewModel;
        }
    }
}
