using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using passocase.Data;
using passo.Service.DTO;
using passo.Service.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace passo.Service
{
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly ILogger<CustomerOrderService> _logger;
        private readonly OrderDbContext _dbContext;

        public CustomerOrderService(IRedisCacheService redisCacheService, ILogger<CustomerOrderService> logger, OrderDbContext dbContext)
        {
            _redisCacheService = redisCacheService;
            _logger = logger;
            _dbContext= dbContext;
        }

        public async Task<int> CreateCustomerOrderAsync(CustomerOrderDTO customerOrderDto)
        {
            if (customerOrderDto.CustomerId==0 || string.IsNullOrWhiteSpace(customerOrderDto.CustomerAddress))
            {
                throw new ArgumentException("CustomerId and CustomerAddress are required.");
            }

            var customerOrder = new CustomerOrder
            {
                CustomerId = customerOrderDto.CustomerId,
                CustomerAddress = customerOrderDto.CustomerAddress
            };

            _dbContext.CustomerOrders.Add(customerOrder);
            await _dbContext.SaveChangesAsync();

            foreach (var productDto in customerOrderDto.Products)
            {
                if (productDto.ProductId <= 0 || productDto.Quantity <= 0)
                {
                    throw new ArgumentException("Invalid product information.");
                }

                var customerOrderDetail = new CustomerOrderDetail
                {
                    ProductId = productDto.ProductId,
                    ProductQuantity = productDto.Quantity,
                    CustomerOrderId = customerOrder.Id 
                };

                _dbContext.CustomerOrderDetails.Add(customerOrderDetail);
            }

            await _dbContext.SaveChangesAsync();
            await UpdateProductListCacheAsync(customerOrder.Id);
            return customerOrder.Id;
        }




        public async Task DeleteCustomerOrderAsync(int orderId)
        {
            var customerOrder = await _dbContext.CustomerOrders
                .FirstOrDefaultAsync(co => co.Id == orderId);

            if (customerOrder == null)
            {
                // Customer order not found
                throw new ArgumentException("Customer order not found.");
            }


            // Delete the customer order
            _dbContext.CustomerOrders.Remove(customerOrder);

            await _dbContext.SaveChangesAsync();
            await UpdateProductListCacheAsync(orderId);
        }

        public async Task UpdateCustomerOrderAsync(int orderId, CustomerOrderDTO customerOrderDto)
        {
            // Retrieve the customer order from the database
            var existingCustomerOrder = await _dbContext.CustomerOrders
                .FirstOrDefaultAsync(co => co.Id == orderId);

            if (existingCustomerOrder == null)
            {
                throw new ArgumentException("Customer order not found.");
            }
            var existingCustomerOrderDetails = _dbContext.CustomerOrderDetails.Where(co => co.CustomerOrderId == orderId).ToList();
            // Update customer information
            existingCustomerOrder.CustomerId = customerOrderDto.CustomerId;
            existingCustomerOrder.CustomerAddress = customerOrderDto.CustomerAddress;

            UpdateProductInformation(existingCustomerOrderDetails, customerOrderDto.Products, orderId);

            await _dbContext.SaveChangesAsync();

            await UpdateProductListCacheAsync(orderId);
        }
        private void UpdateProductInformation(List<CustomerOrderDetail> existingProducts, List<ProductDTO> updatedProducts,int orderId)
        {

         //tüm sipariş etayını sil ve yenden olustur
            _dbContext.CustomerOrderDetails.RemoveRange(existingProducts);

            foreach (var updatedProductDto in updatedProducts)
            {
                var newDetail = new CustomerOrderDetail
                {
                    ProductId = updatedProductDto.ProductId,
                    ProductQuantity = updatedProductDto.Quantity,
                    CustomerOrderId =orderId
                };
                    _dbContext.CustomerOrderDetails.Add(newDetail);
       
            }
        }
        private async Task UpdateProductListCacheAsync(int customerOrderId)
        {
            var productList = await _dbContext.CustomerOrderDetails
                .Where(p => p.CustomerOrderId == customerOrderId)
                .Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    Quantity = p.ProductQuantity
                })
                .ToListAsync();

            await _redisCacheService.SetProductListAsync(productList, customerOrderId);
        }

    }
}

