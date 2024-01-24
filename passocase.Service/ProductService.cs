using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using passocase.Data;
using passo.Service.DTO;
using passo.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace passo.Service
{
    public class ProductService : IProductService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly ILogger<CustomerOrderService> _logger;
        private readonly OrderDbContext _dbContext;

        public ProductService(IRedisCacheService redisCacheService, ILogger<CustomerOrderService> logger, OrderDbContext dbContext)
        {
            _redisCacheService = redisCacheService;
            _logger = logger;
            _dbContext= dbContext;
        }

        public async Task<List<ProductDTO>> GetProductListAsync()
        {
            try
            {
                var productList = await _redisCacheService.GetProductListAsync(0);

                if (productList.Count == 0)
                {
                    var allProducts = (from c in _dbContext.Products

                             select new ProductDTO
                             {
                                 ProductId=c.Id ,
                                 Quantity = c.Quantity 
                             }).ToList() ;
                    productList =  allProducts;
                    await _redisCacheService.SetProductListAsync(productList,0);
                }

                return productList;
            }
            catch (Exception ex)
            {
              
                throw; 
            }
        }
    }
}

