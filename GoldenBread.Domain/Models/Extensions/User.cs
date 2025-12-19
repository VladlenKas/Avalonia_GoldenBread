using GoldenBread.Domain.Enums;
using Humanizer;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenBread.Domain.Models;

public partial class User
{
    [NotMapped] public string Fullname => $"{Lastname} {Firstname} {Patronymic}".Trim() ?? "Неизвестно";

    [NotMapped] public string RoleValue => Role?.Humanize() ?? "Неизвестно";

    [NotMapped] public string VerificationStatusValue => VerificationStatus?.Humanize() ?? "Неизвестно";

    public UserRole? Role { get; set; }

    public AccountType? AccountType { get; set; }

    public VerificationStatus? VerificationStatus { get; set; }
}