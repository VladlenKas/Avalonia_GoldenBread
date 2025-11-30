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
            var request = new LoginRequest { Login = email, Password = password };
            var apiResponse = await apiClient.PostAsync<LoginRequest, ApiResponse<User>>("api/Authorization", request);

            if (apiResponse?.Data != null)
            {
                _currentUser = apiResponse.Data;
            }

            return apiResponse;
        }

        public void Logout()
        {
            _currentUser = null;
            _currentRole = null;
        }
    }
}
