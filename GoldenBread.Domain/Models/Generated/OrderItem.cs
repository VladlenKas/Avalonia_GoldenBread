using System;
using System.Collections.Generic;

namespace GoldenBread.Domain.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int? BatchId { get; set; }

    public int Quantity { get; set; }

    public int? TotalUnits { get; set; }

    public virtual ProductBatch? Batch { get; set; }

    public virtual ICollection<EmployeeTask> EmployeeTasks { get; set; } = new List<EmployeeTask>();

    public virtual Order Order { get; set; } = null!;
}
