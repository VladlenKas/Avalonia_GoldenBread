using GoldenBread.Shared.Entities;
using GoldenBread.Shared.Responses;
using GoldenBread.Shared.Enums.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services
{
    internal class UserService
    {
        public async Task<ApiResponse<User>> LoginAsync(string email, string password)
        {
            var request = new LoginUser { Login = email, Password = password };
            var response = await HttpClientSingleton.Client.PostAsJsonAsync("api/User/login", request);

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<User>>();

            return apiResponse;
        }
    }
}
