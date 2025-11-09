using System;
using System.Collections.Generic;

namespace GoldenBread.Shared.Entities;

public partial class ProductCategory
{
    public int ProductCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public short Deleted { get; set; }

    public string Color { get; set; } = null!;

    public byte[]? Icon { get; set; }

    public byte[]? Image { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
