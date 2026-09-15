using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrdersAPI.Models;
using OrdersAPI.Services.Contracts;
using OrdersAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace OrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService=orderService;
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders=_orderService.GetOrders();
            var ordersResponse = orders.Select(order => new OrderResponse
            {
                Id = order.Id,
                TotalPrice = order.TotalPrice
            }
            );

            var apiResponse = new ApiResponse<IEnumerable<OrderResponse>>
            {
                Data = ordersResponse,
                Message = "Orders retrieved successfully"
               
            };

            return Ok(apiResponse);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById([FromRoute]int id)
        {
            if (id == -1)
                throw new Exception("Failure introduced intentionally");
            try
            {
                var order = _orderService.GetOrderById(id);

                var orderResponse = new OrderResponse
                {
                    Id = order.Id,
                    TotalPrice = order.TotalPrice
                };

                var apiResponse = new ApiResponse<OrderResponse>
                {
                    Data = orderResponse,
                    Message = "Order retrieved successfully"
                };
                return Ok(apiResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }            
      
        }

        [HttpGet("search")]//https://localhost:7125/api/orders/search?status=not_created
        public IActionResult GetOrdersByCustomerId([FromQuery] string status)
        {            
            return Ok(status);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody]CreateOrderRequest request)
        {
            try
            {                
                var order = await _orderService.CreateOrder(request);
                var orderResponse = new OrderResponse
                {
                    Id = order.Id,
                    TotalPrice = order.TotalPrice                  
                };

                var apiResponse = new ApiResponse<OrderResponse>
                {
                    Data = orderResponse,
                    Message = "Order created successfully"
                }; 
                return Ok(apiResponse);

                // return CreatedAtAction(nameof(GetOrderById), new { id = orderResponse.Id }, orderResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}
