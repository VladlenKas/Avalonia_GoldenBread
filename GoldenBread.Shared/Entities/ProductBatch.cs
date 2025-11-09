using System;
using System.Collections.Generic;

namespace GoldenBread.Shared.Entities;

public partial class ProductBatch
{
    public int ProductBatchId { get; set; }

    public int ProductId { get; set; }

    public int Units { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;
}
