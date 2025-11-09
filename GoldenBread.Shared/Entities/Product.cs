using System;
using System.Collections.Generic;

namespace GoldenBread.Shared.Entities;

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
}
