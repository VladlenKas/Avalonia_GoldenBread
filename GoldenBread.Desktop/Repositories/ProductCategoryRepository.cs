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
    public class ProductCategoryRepository(ApiClient apiClient) : IRepository<ProductCategory>
    {
        public async Task<List<ProductCategory>> GetAllAsync()
        {
            var apiResponse = await apiClient.GetAsync<ApiResponse<List<ProductCategory>>>("api/ProductCategories");
            return apiResponse?.Data ?? new List<ProductCategory>();
        }

        public async Task<ApiResponse<object>> DeleteAsync(int id) =>
            await apiClient.DeleteAsync<ApiResponse<object>>($"api/ProductCategories/{id}") ??
            ApiResponse<object>.Failure();

        public async Task<ApiResponse<ProductCategory>> UpdateAsync(ProductCategory category) =>
            await apiClient.PutAsync<ProductCategory, ApiResponse<ProductCategory>>(
                $"api/ProductCategories/{category.ProductCategoryId}", category) ??
            ApiResponse<ProductCategory>.Failure();

        public async Task<ApiResponse<ProductCategory>> CreateAsync(ProductCategory category) =>
            await apiClient.PostAsync<ProductCategory, ApiResponse<ProductCategory>>(
                "api/ProductCategories", category) ??
            ApiResponse<ProductCategory>.Failure();
    }

}
