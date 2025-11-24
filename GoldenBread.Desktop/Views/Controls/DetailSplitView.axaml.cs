using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GoldenBread.Desktop.Interfaces;
using GoldenBread.Desktop.ViewModels.Base;
using GoldenBread.Desktop.ViewModels.Pages;

namespace GoldenBread.Desktop.Views.Controls;

public partial class DetailSplitView : UserControl
{
    // Prop for main pane
    public static readonly StyledProperty<IDataTemplate?> MainContentTemplateProperty =
        AvaloniaProperty.Register<DetailSplitView, IDataTemplate?>(
            nameof(MainContentTemplate));

    public IDataTemplate? MainContentTemplate
    {
        get => GetValue(MainContentTemplateProperty);
        set => SetValue(MainContentTemplateProperty, value);
    }

    // Prop for details pane
    public static readonly StyledProperty<IDataTemplate?> DetailPanelTemplateProperty =
        AvaloniaProperty.Register<DetailSplitView, IDataTemplate?>(
            nameof(DetailPanelTemplate));

    public IDataTemplate? DetailPanelTemplate
    {
        get => GetValue(DetailPanelTemplateProperty);
        set => SetValue(DetailPanelTemplateProperty, value);
    }

    public DetailSplitView()
    {
        InitializeComponent();
    }

    private void SplitView_OnPaneClosing(object? sender, CancelRoutedEventArgs e) 
    {
        if (DataContext is IDetailPanelViewModel vm && vm.IsDetailPanelOpen)
        {
            e.Cancel = true;
        }
    }
}