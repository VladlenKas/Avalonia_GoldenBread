using System;
using System.Collections.Generic;

namespace GoldenBread.Domain.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly Birthday { get; set; }

    public short Deleted { get; set; }

    public virtual ICollection<EmployeeTask> EmployeeTasks { get; set; } = new List<EmployeeTask>();
}
