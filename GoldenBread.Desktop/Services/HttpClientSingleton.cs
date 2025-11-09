using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services
{
    internal class HttpClientSingleton
    {
        public static readonly HttpClient Client = new()
        {
            BaseAddress = new Uri("https://localhost:7153/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        static HttpClientSingleton()
        {
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

    }
}
