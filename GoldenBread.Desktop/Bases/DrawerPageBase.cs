using Avalonia.Controls;
using Avalonia.Input;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.ViewModels.Pages;
using GoldenBread.Domain.Models;
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

    private void OnCardTapped(object sender, TappedEventArgs e)
    {
        if (sender is Control { DataContext: Product product } &&
            DataContext is ProductsPageViewModel vm)
        {
            vm.SelectedItem = product;
        }
    }

}
