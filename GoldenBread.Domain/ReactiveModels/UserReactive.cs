using GoldenBread.Domain.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Domain.ReactiveModels
{
    public class UserReactive : ReactiveObject
    {
        [Reactive] public int UserId { get; set; }
        [Reactive] public string? Firstname { get; set; }
        [Reactive] public string? Lastname { get; set; }
        [Reactive] public string? Patronymic { get; set; }
        [Reactive] public DateOnly? Birthday { get; set; }
        [Reactive] public string? Email { get; set; }
        [Reactive] public string? Password { get; set; }
        [Reactive] public UserRole? Role { get; set; }
        [Reactive] public AccountType? AccountType { get; set; }
        [Reactive] public VerificationStatus? VerificationStatus { get; set; }


        // ==== Computed Props ====
        [ObservableAsProperty] public string Fullname { get; }
        [ObservableAsProperty] public string RoleValue { get; }
        [ObservableAsProperty] public string VerificationStatusValue { get; }

        public UserReactive()
        {
            this.WhenAnyValue(
                    x => x.Firstname,
                    x => x.Lastname,
                    x => x.Patronymic)
                .Select(tuple => $"{tuple.Item2} {tuple.Item1} {tuple.Item3}".Trim())
                .ToPropertyEx(this, x => x.Fullname, initialValue: "Неизвестно");

            this.WhenAnyValue(x => x.Role)
                .Select(role => role?.ToString() ?? "Неизвестно")
                .ToPropertyEx(this, x => x.RoleValue, initialValue: "Неизвестно");

            this.WhenAnyValue(x => x.VerificationStatus)
                .Select(status => status?.ToString() ?? "Неизвестно")
                .ToPropertyEx(this, x => x.VerificationStatusValue, initialValue: "Неизвестно");
        }
    }
}
