using GoldenBread.Desktop.Api;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Repositories
{
    public class ProductRepository(ApiClient apiClient) : IRepository<Product>
    {
        public async Task<List<Product>> GetAllAsync()
        {
            var apiResponse = await apiClient.GetAsync<ApiResponse<List<Product>>>("api/Products");
            return apiResponse?.Data ?? new List<Product>();
        }

        public async Task<ApiResponse<object>> DeleteAsync(int id) =>
            await apiClient.DeleteAsync<ApiResponse<object>>($"api/Products/{id}") ??
            ApiResponse<object>.Failure();

        public async Task<ApiResponse<Product>> UpdateAsync(Product product) =>
            await apiClient.PutAsync<Product, ApiResponse<Product>>($"api/Products/{product.ProductId}", product) ??
            ApiResponse<Product>.Failure();

        public async Task<ApiResponse<Product>> CreateAsync(Product product) =>
            await apiClient.PostAsync<Product, ApiResponse<Product>>("api/Products", product) ??
            ApiResponse<Product>.Failure();
    }

}
