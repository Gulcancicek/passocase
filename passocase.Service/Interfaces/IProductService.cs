using passo.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace passo.Service.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetProductListAsync();
    }
}
