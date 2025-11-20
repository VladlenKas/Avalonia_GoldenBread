using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Avalonia;

namespace GoldenBread.Desktop.Views;

public partial class MenuView : ReactiveWindow<MenuViewModel>
{
    public MenuView()
    {
        InitializeComponent();
    }
}