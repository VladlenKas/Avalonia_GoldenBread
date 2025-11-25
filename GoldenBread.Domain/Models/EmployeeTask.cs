using System;
using System.Collections.Generic;

namespace GoldenBread.Domain.Models;

public partial class EmployeeTask
{
    public int EmployeeTaskId { get; set; }

    public int EmployeeId { get; set; }

    public int OrderItemId { get; set; }

    public int AssignedQuantity { get; set; }

    public int? CompletedQuantity { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual OrderItem OrderItem { get; set; } = null!;
}
