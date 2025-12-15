using Avalonia.Controls;
using Avalonia.Input;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Bases;

public partial class UserControlBase : UserControl
{
    // For fast closing detail panel on the pages
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.Source is Border border && border.Name is "ShadingBorder")   
        {
            if (DataContext is IPageViewModel vm)
            {
                vm.SelectedItem = default;
                vm.DetailsPanel.CloseCommand?.Execute(null);
            }
        }
    }
}
