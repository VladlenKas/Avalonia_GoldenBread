using System;
using System.Collections.Generic;

namespace GoldenBread.Shared.Entities;

public partial class Recipe
{
    public int RecipeId { get; set; }

    public int ProductId { get; set; }

    public int IngredientId { get; set; }

    public decimal Quantity { get; set; }

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
