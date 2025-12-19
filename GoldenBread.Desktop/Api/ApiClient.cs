using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Api
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient, ApiSettings settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(
            string endpoint,
            TRequest data)
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task<T?> DeleteAsync<T>(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
