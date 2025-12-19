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
    public class ProductCategoryService(IRepository<ProductCategory> repository) : IService<ProductCategory>
    {
        public ProductCategory Clone(ProductCategory entity)
        {
            return new ProductCategory
            {
                ProductCategoryId = entity.ProductCategoryId,
                Name = entity.Name,
                Deleted = entity.Deleted,
                Color = entity.Color,
                Icon = entity.Icon,
                Image = entity.Image
            };
        }

        public async Task<List<ProductCategory>> GetAllAsync()
        {
            return await repository.GetAllAsync();
        }

        public async Task<ApiResponse<ProductCategory>> SaveAsync(ProductCategory entity, bool isNew)
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

        public async Task<ApiResponse<object>> DeleteAsync(ProductCategory entity)
        {
            return await repository.DeleteAsync(entity.ProductCategoryId);
        }
    }

}
