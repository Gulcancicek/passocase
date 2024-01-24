using System.Collections.Generic;

namespace passo.Service.DTO
{
    public class CustomerOrderDTO
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerAddress { get; set; }

        public List<ProductDTO> Products { get; set; }
    }
}
