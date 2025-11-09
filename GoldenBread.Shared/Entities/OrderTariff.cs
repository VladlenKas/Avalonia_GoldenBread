using System;
using System.Collections.Generic;

namespace GoldenBread.Shared.Entities;

public partial class OrderTariff
{
    public int OrderTariffId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal MarkupPercent { get; set; }

    public decimal FreeEmployeesPercent { get; set; }

    public short Deleted { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
