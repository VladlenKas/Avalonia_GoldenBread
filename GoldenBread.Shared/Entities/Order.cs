using System;
using System.Collections.Generic;

namespace GoldenBread.Shared.Entities;

public enum OrderStatus
{
    Awaiting,
    InProgress,
    Completed,
    Canceled
}

public partial class Order
{
    public OrderStatus Status { get; set; }

    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int TariffId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual OrderTariff Tariff { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
