using GoldenBread.Desktop.Repositories;
using GoldenBread.Domain.Models;
using GoldenBread.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Services
{
    public class ProductService(IRepository<Product> repository) : IService<Product>
    {
        public Product Clone(Product entity)
        {
            return new Product
            {
                ProductId = entity.ProductId,
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Description = entity.Description,
                CostPrice = entity.CostPrice,
                SalePrice = entity.SalePrice,
                MarkupPercent = entity.MarkupPercent,
                Weight = entity.Weight,
                ProductionTime = entity.ProductionTime,
                Deleted = entity.Deleted,
                Category = entity.Category,
                ProductImages = entity.ProductImages
            };
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await repository.GetAllAsync();
        }

        public async Task<ApiResponse<Product>> SaveAsync(Product entity, bool isNew)
        {
            if (isNew)
            {
                return await repository.CreateAsync(entity);
            }
            else
            {
                return await repository.UpdateAsync(entity);
            }
        }

        public async Task<ApiResponse<object>> DeleteAsync(Product entity)
        {
            return await repository.DeleteAsync(entity.ProductId);
        }
    }

}
