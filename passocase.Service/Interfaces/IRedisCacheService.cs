using passo.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace passo.Service.Interfaces
{
    public interface IRedisCacheService
    {
        Task<List<ProductDTO>> GetProductListAsync(int orderId);
        Task SetProductListAsync(List<ProductDTO> productList, int orderId);
    }
}
