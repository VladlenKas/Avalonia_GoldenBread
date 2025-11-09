using System;
using System.Collections.Generic;

namespace GoldenBread.Shared.Entities;

public enum IngredientBatchStatus
{
    Available,
    Expired,
    OutOfStock
}

public partial class IngredientBatch
{
    public IngredientBatchStatus Status { get; set; }

    public int IngredientBatchId { get; set; }

    public int IngredientId { get; set; }

    public int PurchasedQuantity { get; set; }

    public decimal RemainingQuantity { get; set; }

    public DateOnly DeliveryDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public virtual Ingredient Ingredient { get; set; } = null!;
}
