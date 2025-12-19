using GoldenBread.Api.Helpers;
using GoldenBread.Api.Services;
using GoldenBread.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBread.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromServices] ProductCategoryService service)
        {
            try
            {
                var categories = await service.GetAllAsync();
                return SuccessWithData(categories, MessageHelper.SuccessFromApi);
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromServices] ProductCategoryService service, int id)
        {
            try
            {
                var result = await service.DeleteAsync(id);
                if (!result)
                {
                    return NotFoundError("Категория не найдена");
                }

                return Success<object>("Категория удалена");
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromServices] ProductCategoryService service, [FromBody] ProductCategory category)
        {
            try
            {
                var created = await service.CreateAsync(category);
                return SuccessWithData(created, "Категория создана");
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromServices] ProductCategoryService service, int id, [FromBody] ProductCategory category)
        {
            try
            {
                var result = await service.UpdateAsync(id, category);
                if (result == null)
                {
                    return NotFoundError("Категория не найдена");
                }

                return SuccessWithData(result, "Категория обновлена");
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }
    }
}
