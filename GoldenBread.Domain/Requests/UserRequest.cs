using GoldenBread.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Domain.Requests
{
    public class UserRequest
    {
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
    }
}
