using Avalonia.Controls;
using Avalonia.Input;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.ViewModels.Pages;

namespace GoldenBread.Desktop.Bases;

public partial class DetailsPanelOverlayPage : UserControl
{
    // Для быстрого закрытия панели
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.Source is Border border && border.Name is "ShadingBorder")   
        {
            if (DataContext is UsersPageViewModel vm && vm.IsViewMode)
            {
                vm.CancelCommand.Execute();
            }
        }
    }
}
