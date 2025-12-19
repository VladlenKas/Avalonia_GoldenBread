using GoldenBread.Api.Helpers;
using GoldenBread.Api.Services;
using GoldenBread.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoldenBread.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromServices] ProductService service)
        {
            try
            {
                var products = await service.GetAllAsync();
                return SuccessWithData(products, MessageHelper.SuccessFromApi);
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromServices] ProductService service, int id)
        {
            try
            {
                var result = await service.DeleteAsync(id);
                if (!result)
                {
                    return NotFoundError(MessageHelper.ProductNotFound);
                }

                return Success<object>(MessageHelper.ProductDeleted);
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromServices] ProductService service, [FromBody] Product product)
        {
            try
            {
                var createdProduct = await service.CreateAsync(product);
                return SuccessWithData(createdProduct, MessageHelper.ProductCreated);
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromServices] ProductService service, int id, [FromBody] Product product)
        {
            try
            {
                var result = await service.UpdateAsync(id, product);
                if (result == null)
                {
                    return NotFoundError(MessageHelper.ProductNotFound);
                }

                return SuccessWithData(result, MessageHelper.ProductUpdated);
            }
            catch (Exception ex)
            {
                return ServerError(MessageHelper.ErrorFromApi);
            }
        }
    }

}
