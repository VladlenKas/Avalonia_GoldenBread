using Avalonia.Controls;
using Avalonia.Input;
using GoldenBread.Desktop.Interfaces;
using System.Reactive;

namespace GoldenBread.Desktop.Bases;

public partial class DrawerPageBase : UserControl
{
    // Для быстрого закрытия панели
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.Source is Border border && border.Name is "ShadingBorder")   
        {
            if (DataContext is IDrawerViewModel vm)
            {
                vm.CloseCommand.Execute().Subscribe(Observer.Create<Unit>(_ => { }));
            }
        }
    }
}
