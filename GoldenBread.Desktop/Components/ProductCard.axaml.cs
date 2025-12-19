using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace GoldenBread.Desktop.Components;

public partial class ProductCard : UserControl
{
    public ProductCard()
    {
        InitializeComponent();

        // Адаптивность на основе размера
        this.GetObservable(BoundsProperty)
            .Subscribe(bounds => AdaptLayout(bounds.Width));
    }

    private void AdaptLayout(double width)
    {
        var cardGrid = this.FindControl<Grid>("CardGrid");
        if (cardGrid == null) return;

        if (width > 400)
        {
            // Горизонтальный макет
            cardGrid.RowDefinitions.Clear();
            cardGrid.ColumnDefinitions.Clear();
            cardGrid.ColumnDefinitions = new ColumnDefinitions("200,*");

            var imageContainer = cardGrid.Children[0] as Border;
            if (imageContainer != null)
            {
                Grid.SetColumn(imageContainer, 0);
                Grid.SetRowSpan(imageContainer, 4);
                imageContainer.Height = double.NaN;
            }

            for (int i = 1; i < cardGrid.Children.Count; i++)
            {
                Grid.SetColumn(cardGrid.Children[i], 1);
                Grid.SetRow(cardGrid.Children[i], i - 1);
            }
        }
        else
        {
            // Вертикальный макет
            cardGrid.ColumnDefinitions.Clear();
            cardGrid.RowDefinitions.Clear();
            cardGrid.RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto");

            foreach (var child in cardGrid.Children)
            {
                Grid.SetColumn(child, 0);
                Grid.SetRow(child, cardGrid.Children.IndexOf(child));
                Grid.SetRowSpan(child, 1);
            }
        }
    }

}