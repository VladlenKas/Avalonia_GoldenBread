using GoldenBread.Desktop.ViewModels.Pages;
using GoldenBread.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.Mappers
{
    public class ProductMapper : IMapper<Product>
    {
        public void MapEntityToViewModel(Product entity, object viewModel)
        {
            if (viewModel is ProductsPageViewModel vm)
            {
                vm.EditName = entity.Name;
                vm.EditDescription = entity.Description;
                vm.EditCostPrice = entity.CostPrice;
                vm.EditSalePrice = entity.SalePrice;
                vm.EditMarkupPercent = entity.MarkupPercent;
                vm.EditWeight = entity.Weight;
                vm.EditProductionTime = entity.ProductionTime;
                vm.EditCategory = vm.AvailableCategories.FirstOrDefault(c => c.ProductCategory == entity.Category);

                // Загружаем изображения
                vm.EditImages.Clear();
                foreach (var img in entity.ProductImages)
                {
                    vm.EditImages.Add(new ProductImageViewModel
                    {
                        ImageData = img.Image,
                        IsNew = false
                    });
                }
            }
        }

        public Product MapEntityFromViewModel(object viewModel, Product? existingEntity = null)
        {
            var product = existingEntity ?? new Product();

            if (viewModel is ProductsPageViewModel vm)
            {
                product.Name = vm.EditName;
                product.Description = vm.EditDescription;
                product.CostPrice = vm.EditCostPrice;
                product.SalePrice = vm.EditSalePrice;
                product.MarkupPercent = vm.EditMarkupPercent;
                product.Weight = vm.EditWeight;
                product.ProductionTime = vm.EditProductionTime;
                product.Category = vm.EditCategory?.ProductCategory!;

                // Добавляем только новые изображения
                if (existingEntity == null)
                {
                    product.ProductImages = new List<ProductImage>();
                }

                foreach (var imgVm in vm.EditImages.Where(i => i.IsNew))
                {
                    product.ProductImages.Add(new ProductImage
                    {
                        Image = imgVm.ImageData
                    });
                }
            }

            return product;
        }
    }
}
