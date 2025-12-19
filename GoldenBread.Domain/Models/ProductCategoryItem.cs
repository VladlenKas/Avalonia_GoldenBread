using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBread.Domain.Models
{
    public class ProductCategoryItem
    {
        public ProductCategory ProductCategory { get; set; }
        public string DisplayName => ProductCategory.Name.Humanize();
    }
}
