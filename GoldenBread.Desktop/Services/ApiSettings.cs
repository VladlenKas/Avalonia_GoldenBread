using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = "https://localhost:7153/";
        public int TimeoutSeconds { get; set; } = 30;
    }
}
