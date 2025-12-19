using Humanizer;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenBread.Domain.Models;

partial class Product
{
    // Первое изображение для отображения в карточке
    [NotMapped] public byte[]? FirstImage => ProductImages.FirstOrDefault(x => x.ProductImageId == ProductId)?.Image;

    // Тренд продаж (изменение в процентах)
    [NotMapped]
    public decimal TrendPercent
    {
        get
        {
            var now = DateTime.Now;
            var currentMonth = new DateOnly(now.Year, now.Month, 1);
            var previousMonth = currentMonth.AddMonths(-1);

            // Продажи текущего месяца
            var currentSales = ProductBatches
                .SelectMany(pb => pb.OrderItems)
                .Where(oi => oi.Order.StartDate >= currentMonth)
                .Sum(oi => oi.Quantity);

            // Продажи предыдущего месяца
            var previousSales = ProductBatches
                .SelectMany(pb => pb.OrderItems)
                .Where(oi => oi.Order.StartDate >= previousMonth &&
                            oi.Order.StartDate < currentMonth)
                .Sum(oi => oi.Quantity);

            if (previousSales == 0)
                return currentSales > 0 ? 100 : 0;

            return Math.Round((decimal)(currentSales - previousSales) / previousSales * 100, 1);
        }
    }

    // Форматированный тренд для отображения
    [NotMapped]
    public string TrendFormatted
    {
        get
        {
            var trend = TrendPercent;
            var sign = trend >= 0 ? "+" : "";
            return $"{sign}{trend:F1}%";
        }
    }
}

public class ProductCategoryItem
{
    public ProductCategory ProductCategory { get; set; }
    public string DisplayName => ProductCategory.Name.Humanize();
}