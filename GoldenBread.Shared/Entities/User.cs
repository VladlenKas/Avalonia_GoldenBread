using Humanizer;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenBread.Shared.Entities;

public enum AccountType
{
    [Description("Пользователь")]
    User,
    [Description("Компания")]
    Company
}

public enum UserRole
{
    [Description("Менеджер производства")]
    ManagerProduction,
    [Description("Администратор")]
    Admin
}
    
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


public partial class User 
{
    // == Custom props ==
    [NotMapped]
    public string Fullname => $"{Lastname} {Firstname} {Patronymic}";

    [NotMapped]
    public string RoleValue => Role.Value.Humanize();

    [NotMapped]
    public string VerificationStatusValue => VerificationStatus.Value.Humanize();


    // == Original props ==
    public UserRole? Role { get; set; } 

    public AccountType? AccountType { get; set; }

    public VerificationStatus? VerificationStatus { get; set; }

    public int UserId { get; set; } 

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Patronymic { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public short Dismissed { get; set; }

    public string? CompanyName { get; set; }

    public string? CompanyInn { get; set; }

    public string? CompanyOgrn { get; set; }

    public string? CompanyPhone { get; set; }

    public string? CompanyAddress { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
