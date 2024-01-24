using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using passocase.Data;
using passo.Service.DTO;
using passo.Service.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace passo.Service
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly OrderDbContext _dbContext;
        public RedisCacheService(IConfiguration configuration, OrderDbContext dbContext)
        {
            _configuration = configuration;
            var redisConnectionString = _configuration.GetValue<string>("RedisCache:ConnectionString");
            _redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
            _dbContext=dbContext;
        }



        public async Task<List<ProductDTO>> GetProductListAsync(int orderId)
        {

            try
            {
                var redisDatabase = _redisConnection.GetDatabase();
                var cachedProductList = await redisDatabase.StringGetAsync(GetProductListCacheKey(orderId));

                if (!cachedProductList.IsNullOrEmpty)
                {
                    // Cache hit
                    return JsonSerializer.Deserialize<List<ProductDTO>>(cachedProductList);
                }
                return new List<ProductDTO>();
            }
            catch (Exception ex)
            {
                throw; // Propagate the exception to the caller
            }


        }
        public async Task SetProductListAsync(List<ProductDTO> productList, int orderId)
        {

            try
            {
                var redisDatabase = _redisConnection.GetDatabase();

                // Serialize the product list to JSON
                var serializedProductList = JsonSerializer.Serialize(productList);

                // Set the product list in Redis cache with an expiration time
                await redisDatabase.StringSetAsync(GetProductListCacheKey(orderId), serializedProductList, TimeSpan.FromMinutes(_configuration.GetValue<int>("RedisCache:ExpirationMinutes")));
            }
            catch (Exception ex)
            {
                // Log the exception
                throw; // Propagate the exception to the caller
            }
        }


        private string GetProductListCacheKey(int orderId)
        {
            if (orderId == 0)
            return "ProductList_";
            return "ProductList_"+orderId;
        }
    }
}
