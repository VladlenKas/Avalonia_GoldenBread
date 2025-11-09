using System;
using System.Collections.Generic;

namespace GoldenBread.Shared.Entities;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int UserId { get; set; }

    public int? BatchId { get; set; }

    public int Quantity { get; set; }

    public virtual ProductBatch? Batch { get; set; }

    public virtual User User { get; set; } = null!;
}
