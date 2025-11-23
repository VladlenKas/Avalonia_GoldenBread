using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.ViewModels.Pages;
using System;
using System.ComponentModel;

namespace GoldenBread.Desktop.Views.Pages;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();
    }

    private void DataGrid_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is UsersViewModel vm && vm.SelectedItem != null)
        {
            vm.ViewCommand.Execute(vm.SelectedItem).Subscribe();
        }
    }

    private void SplitView_OnPaneClosing(object? sender, CancelRoutedEventArgs e)
    {
        if (DataContext is UsersViewModel vm && vm.IsDetailPanelOpen)
        {
            e.Cancel = true;
        }
    }
}