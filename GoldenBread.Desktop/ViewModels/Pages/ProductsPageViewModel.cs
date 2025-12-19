using DynamicData;
using GoldenBread.Desktop.Bases;
using GoldenBread.Desktop.Mappers;
using GoldenBread.Desktop.Services;
using GoldenBread.Domain.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GoldenBread.Desktop.ViewModels.Pages
{
    public class ProductsPageViewModel : PageViewModelBase<Product>
    {
        // Поля для редактирования
        [Reactive] public string EditName { get; set; } = string.Empty;
        [Reactive] public string EditDescription { get; set; } = string.Empty;
        [Reactive] public decimal EditCostPrice { get; set; }
        [Reactive] public decimal EditSalePrice { get; set; }
        [Reactive] public int EditMarkupPercent { get; set; }
        [Reactive] public decimal EditWeight { get; set; }
        [Reactive] public int EditProductionTime { get; set; }
        [Reactive] public ProductCategoryItem EditCategory { get; set; }

        // Коллекция изображений для редактирования
        [Reactive] public ObservableCollection<ProductImageViewModel> EditImages { get; set; } = new();

        public IEnumerable<ProductCategoryItem> AvailableCategories { get; set; }

        private readonly IService<ProductCategory> _categoryService;

        public ProductsPageViewModel(
            IService<Product> productCrudService,
            IService<ProductCategory> categoryService,
            IMapper<Product> mapper)
            : base(p => p.ProductId, productCrudService, mapper)
        {
            _categoryService = categoryService;

            // Загрузка категорий
            AvailableCategories = Task.Run(() => _categoryService.GetAllAsync()).Result
                .Select(category => new ProductCategoryItem { ProductCategory = category })
                .ToList();

            // Валидация
            ValidateRequired(this, vm => vm.EditName);
            /*this.ValidateRequired(this, vm => vm.EditCostPrice);
            this.ValidateRequired(this, vm => vm.EditSalePrice);
            this.ValidateRequired(this, vm => vm.EditWeight);
            this.ValidateRequired(this, vm => vm.EditCategory);*/

            Initialize();
        }

        protected override void ClearEditFields()
        {
            EditName = string.Empty;
            EditDescription = string.Empty;
            EditCostPrice = 0;
            EditSalePrice = 0;
            EditMarkupPercent = 0;
            EditWeight = 0;
            EditProductionTime = 0;
            EditCategory = null;
            EditImages.Clear();
        }

        public override async Task LoadDataAsync()
        {
            var products = await _service.GetAllAsync();
            _sourceCache.Clear();
            _sourceCache.AddOrUpdate(products);
        }
    }

    public class ProductImageViewModel : ReactiveObject
    {
        [Reactive] public byte[] ImageData { get; set; } = Array.Empty<byte>();
        [Reactive] public bool IsNew { get; set; }
    }
}
