using GoldenBread.Domain.Enums;

namespace GoldenBread.Domain.Models;

public partial class Order
{
    public OrderStatus Status { get; set; }
}
