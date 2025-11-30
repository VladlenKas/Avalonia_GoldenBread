using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Domain.Models
{
    public class VerificationStatusItem
    {
        public VerificationStatus Status { get; set; }
        public string DisplayName => Status.Humanize();
    }
}
