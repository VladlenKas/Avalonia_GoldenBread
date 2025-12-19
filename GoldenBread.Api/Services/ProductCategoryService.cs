using GoldenBread.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Api.Services
{
    public class ProductCategoryService(GoldenBreadContext context)
    {
        public async Task<List<ProductCategory>> GetAllAsync()
        {
            return await context.ProductCategories
                .Where(c => c.Deleted == 0)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProductCategory> CreateAsync(ProductCategory request)
        {
            context.ProductCategories.Add(request);
            await context.SaveChangesAsync();
            return request;
        }

        public async Task<ProductCategory?> UpdateAsync(int id, ProductCategory request)
        {
            var existing = await context.ProductCategories.FindAsync(id);
            if (existing == null)
                return null;

            existing.Name = request.Name;
            existing.Color = request.Color;
            existing.Icon = request.Icon;
            existing.Image = request.Image;

            await context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await context.ProductCategories.FindAsync(id);
            if (category == null)
                return false;

            category.Deleted = 1;
            context.ProductCategories.Update(category);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
