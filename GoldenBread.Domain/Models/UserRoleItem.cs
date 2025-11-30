using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Domain.Models
{
    public class UserRoleItem
    {
        public UserRole Role { get; set; }
        public string DisplayName => Role.Humanize();
    }
}
