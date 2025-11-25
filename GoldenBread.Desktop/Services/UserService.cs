using GoldenBread.Desktop.Helpers;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.Responses;
using System;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services
{
    public class UserService(ApiClient apiClient)
    {
        public async Task<List<User>> GetAllAsync()
        {
            var apiResponse = await apiClient.GetAsync<ApiResponse<List<User>>>("api/Users");
            return apiResponse?.Data ?? new List<User>();
        }

        public async Task<ApiResponse<object>> DeleteAsync(int id)
        {
            return await apiClient.DeleteAsync<ApiResponse<object>>($"api/Users/{id}");
        }

        public async Task<ApiResponse<User>> UpdateAsync(User user)
        {
            return await apiClient.PutAsync<User, ApiResponse<User>>($"api/Users/{user.UserId}", user);
        }

        public async Task<ApiResponse<User>> CreateAsync(User user)
        {
            return await apiClient.PostAsync<User, ApiResponse<User>>("api/Users", user);
        }

    }
}
