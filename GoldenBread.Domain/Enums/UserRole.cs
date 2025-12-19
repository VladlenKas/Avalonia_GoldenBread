using Humanizer;
using System.ComponentModel;

namespace GoldenBread.Domain.Enums;

public enum UserRole
{
    [Description("Менеджер производства")]
    ManagerProduction,
    [Description("Администратор")]
    Admin
}

public class UserRoleItem
{
    public UserRole Role { get; set; }
    public string DisplayName => Role.Humanize();
}