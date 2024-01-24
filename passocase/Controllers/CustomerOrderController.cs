using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using passo.Service.DTO;
using passo.Service.Interfaces;
using System.Threading.Tasks;
using System;
using System.Net;
using Swashbuckle.AspNetCore.Annotations;

namespace passocase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerOrderController : ControllerBase
    {
        private readonly ICustomerOrderService _customerOrderService;

        public CustomerOrderController(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }

        [SwaggerResponse((int)HttpStatusCode.OK, "Creates Order", typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(
Summary = "Creates Order",
Description = "Creates Order",
OperationId = "CreateCustomerOrder")]
        [HttpPost("customerorders")]
        public async Task<IActionResult> CreateCustomerOrder([FromBody] CustomerOrderDTO customerOrderDto)
        {
            try
            {
                var orderId = await _customerOrderService.CreateCustomerOrderAsync(customerOrderDto);
                return Ok(new { OrderId = orderId });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }
        [SwaggerResponse((int)HttpStatusCode.OK, "Deletes Order", typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(
Summary = "Deletes Order",
Description = "Deletes Order",
OperationId = "DeleteCustomerOrder")]
        [HttpDelete("customerorders/{orderId}")]
        public async Task<IActionResult> DeleteCustomerOrder(int orderId)
        {
            try
            {
                await _customerOrderService.DeleteCustomerOrderAsync(orderId);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }

        [HttpPut("customerorders/{orderId}")]
        public async Task<IActionResult> UpdateCustomerOrder(int orderId, [FromBody] CustomerOrderDTO customerOrderDto)
        {
            try
            {
                await _customerOrderService.UpdateCustomerOrderAsync(orderId, customerOrderDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }
    }
}
