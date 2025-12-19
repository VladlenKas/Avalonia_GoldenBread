using System.ComponentModel;

namespace GoldenBread.Domain.Enums;

public enum AccountType
{
    [Description("Пользователь")]
    User,
    [Description("Компания")]
    Company
}
