using System;
using System.Collections.Generic;

namespace GoldenBread.Domain.Models;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public int SupplierId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public short Deleted { get; set; }

    public decimal Weight { get; set; }

    public int ShelfLifeMonths { get; set; }

    public virtual ICollection<IngredientBatch> IngredientBatches { get; set; } = new List<IngredientBatch>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public virtual Supplier Supplier { get; set; } = null!;
}
