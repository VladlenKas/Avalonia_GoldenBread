using GoldenBread.Shared.Entities;
using GoldenBread.Shared.Responses;
using GoldenBread.Shared.Requests;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using GoldenBread.Desktop.Helpers;

namespace GoldenBread.Desktop.Services
{
    public class AuthorizationService
    {
        // == Fields ==
        private User? _currentUser;


        // == Props ==
        public User? CurrentUser
        {
            get => _currentUser;
            private set => _currentUser = value;
        }

        public bool IsAuthenticated => _currentUser != null;


        // == Methods ==
        public async Task<ApiResponse<User>> LoginAsync(string email, string password)
        {
            var request = new LoginUser { Login = email, Password = password };
            var response = await HttpClientHelper.Client.PostAsJsonAsync("api/Authorization/login", request);
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<User>>();

            if (apiResponse?.Data != null)
            {
                _currentUser = apiResponse.Data;
            }

            return apiResponse;
        }

        public void Logout()
        {
            _currentUser = null;
        }
    }
}
