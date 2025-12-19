using GoldenBread.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenBread.Api.Services
{
    public class ProductService(GoldenBreadContext context)
    {
        // Get List с включением связанных данных
        public async Task<List<Product>> GetAllAsync()
        {
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductBatches)
                    .ThenInclude(pb => pb.OrderItems)
                        .ThenInclude(oi => oi.Order)
                .Where(p => p.Deleted == 0)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product request)
        {
            /*var existingCategory = await context.ProductCategories
                .FindAsync(request.Category.ProductCategoryId);

            if (existingCategory == null)
            {
                throw new Exception("Категория не найдена");
            }
            var category = await context.ProductCategories
                .FindAsync(request.CategoryId);

            request.Category = null;
            request.CategoryId = 5;*/

            context.Products.Add(request);
            await context.SaveChangesAsync();

            // Загружаем связанные данные для возврата
            await context.Entry(request).Reference(p => p.Category).LoadAsync();
            await context.Entry(request).Collection(p => p.ProductImages).LoadAsync();

            return request;
        }

        public async Task<Product?> UpdateAsync(int id, Product request)
        {
            var existingProduct = await context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (existingProduct == null)
                return null;

            existingProduct.Name = request.Name;
            existingProduct.Description = request.Description;
            existingProduct.CostPrice = request.CostPrice;
            existingProduct.SalePrice = request.SalePrice;
            existingProduct.MarkupPercent = request.MarkupPercent;
            existingProduct.Weight = request.Weight;
            existingProduct.ProductionTime = request.ProductionTime;
            existingProduct.CategoryId = request.CategoryId;

            // Обновляем изображения (добавляем новые)
            if (request.ProductImages != null && request.ProductImages.Any())
            {
                foreach (var newImage in request.ProductImages.Where(i => i.ProductImageId == 0))
                {
                    existingProduct.ProductImages.Add(newImage);
                }
            }

            await context.SaveChangesAsync();

            // Перезагружаем связанные данные
            await context.Entry(existingProduct).Reference(p => p.Category).LoadAsync();

            return existingProduct;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null)
                return false;

            // Мягкое удаление
            product.Deleted = 1;
            context.Products.Update(product);
            await context.SaveChangesAsync();
            return true;
        }
    }

}
