using GoldenBread.Desktop.Helpers;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.Responses;
using System;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldenBread.Domain.Requests;

namespace GoldenBread.Desktop.Services.Api
{
    public class UserApiService(ApiClient apiClient) : IApiService<User>
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
            var dto = MapToRequest(user);
            return await apiClient.PutAsync<UserRequest, ApiResponse<User>>($"api/Users/{user.UserId}", dto);
        }

        public async Task<ApiResponse<User>> CreateAsync(User user)
        {
            var dto = MapToRequest(user);
            return await apiClient.PostAsync<UserRequest, ApiResponse<User>>("api/Users", dto);
        }

        private static UserRequest MapToRequest(User user) => new()
        {
            UserId = user.UserId,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Patronymic = user.Patronymic,
            Birthday = user.Birthday,
            Email = user.Email,
            Password = user.Password,
            Role = user.Role,
            AccountType = user.AccountType,
            VerificationStatus = user.VerificationStatus,
            Dismissed = user.Dismissed
        };
    }
}
