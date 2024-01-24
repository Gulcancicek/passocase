using passo.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace passo.Service.Interfaces
{
    public interface ICustomerOrderService
    {
        Task<int> CreateCustomerOrderAsync(CustomerOrderDTO customerOrderDto);
        Task DeleteCustomerOrderAsync(int orderId);
        Task UpdateCustomerOrderAsync(int orderId, CustomerOrderDTO customerOrderDto);
    }
}
