using GoldenBread.Desktop.Helpers;
using GoldenBread.Shared.Entities;
using GoldenBread.Shared.Responses;
using System;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services
{
    public class UserService
    {
        public async Task<List<User>> GetAllAsync()
        {
            var response = await HttpClientHelper.Client.GetAsync("api/Users/getList");
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<User>>>();
            return apiResponse?.Data ?? new List<User>();
        }
    }
}
