using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using passo.Service.Interfaces;
using System.Threading.Tasks;
using System;

namespace passocase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductList()
        {
            try
            {
                var productList = await _productService.GetProductListAsync();

                return Ok(productList);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }
    }
}
