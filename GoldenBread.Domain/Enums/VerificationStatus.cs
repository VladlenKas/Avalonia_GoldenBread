using Humanizer;
using System.ComponentModel;

namespace GoldenBread.Domain.Enums;

public enum VerificationStatus
{
    [Description("Ожидает подтверждения")]
    Pending,
    [Description("Подтвержден")]
    Approved,
    [Description("Отклонен")]
    Rejected,
    [Description("Заморожен")]
    Suspended
}

public class VerificationStatusItem
{
    public VerificationStatus Status { get; set; }
    public string DisplayName => Status.Humanize();
}
