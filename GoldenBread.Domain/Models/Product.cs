using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenBread.Domain.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal CostPrice { get; set; }

    public decimal SalePrice { get; set; }

    public int MarkupPercent { get; set; }

    public decimal Weight { get; set; }

    public int ProductionTime { get; set; }

    public short Deleted { get; set; }

    public virtual ProductCategory Category { get; set; } = null!;

    public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();

    public virtual ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    // Первое изображение для отображения в карточке
    [NotMapped] public byte[]? FirstImage => ProductImages.FirstOrDefault(x => x.ProductImageId == this.ProductId)?.Image;

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

            return Math.Round(((decimal)(currentSales - previousSales) / previousSales) * 100, 1);
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
