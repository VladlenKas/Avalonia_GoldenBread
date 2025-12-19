using GoldenBread.Desktop.Api;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.Requests;
using GoldenBread.Domain.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Repositories
{
    public class UserRepository(ApiClient apiClient) : IRepository<User>
    {
        public async Task<List<User>> GetAllAsync()
        {
            var apiResponse = await apiClient.GetAsync<ApiResponse<List<User>>>("api/Users");
            return apiResponse?.Data ?? new List<User>();
        }

        public async Task<ApiResponse<object>> DeleteAsync(int id) =>
            await apiClient.DeleteAsync<ApiResponse<object>>($"api/Users/{id}") ??
            ApiResponse<object>.Failure();

        public async Task<ApiResponse<User>> UpdateAsync(User user) =>
            await apiClient.PutAsync<User, ApiResponse<User>>($"api/Users/{user.UserId}", user) ??
            ApiResponse<User>.Failure();

        public async Task<ApiResponse<User>> CreateAsync(User user) =>
            await apiClient.PostAsync<User, ApiResponse<User>>("api/Users", user) ??
            ApiResponse<User>.Failure();
    }
}
