using GoldenBread.Desktop.Enums;
using GoldenBread.Desktop.Helpers;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.Requests;
using GoldenBread.Domain.Responses;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services
{
    public class AuthorizationService(ApiClient apiClient)
    {
        // == Fields ==
        private User? _currentUser;
        private UserRole? _currentRole;

        // == Props ==
        public User? CurrentUser
        {
            get => _currentUser;
            private set => _currentUser = value;
        }

        public UserRole? CurrentRole
        {
            get => _currentUser.Role;
        }

        public bool IsAuthenticated => _currentUser != null;


        // == Methods ==
        public async Task<ApiResponse<User>> LoginAsync(string email, string password)
        {
            var request = new LoginUser { Login = email, Password = password };
            var apiResponse = await apiClient.PostAsync<LoginUser, ApiResponse<User>>("api/Authorization", request);

            if (apiResponse?.Data != null)
            {
                _currentUser = apiResponse.Data;
            }

            return apiResponse;
        }

        /// <summary>
        /// A function for granting rights depending on the position
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermission(Permission permission)
        {
            return CurrentRole switch
            {
                UserRole.Admin => true,
                UserRole.ManagerProduction => permission != Permission.Delete,
                _ => false
            };
        }

        public void Logout()
        {
            _currentUser = null;
            _currentRole = null;
        }
    }
}
