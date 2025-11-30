using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.ViewModels.Pages;
using System;

namespace GoldenBread.Desktop.Views.Pages;

public partial class UsersPageView : UserControl
{
    public UsersPageView()
    {
        InitializeComponent();
    }

    private void OnOverlayClick(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is UsersPageViewModel vm)
        {
            vm.DetailsPanel.CloseCommand?.Execute().Subscribe();
        }
    }
}