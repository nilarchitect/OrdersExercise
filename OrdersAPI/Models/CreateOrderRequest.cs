using System.ComponentModel.DataAnnotations;
using OrdersAPI.Models.DTOs;

namespace OrdersAPI.Models
{
    public class CreateOrderRequest
    {
    //    [Required]
    //    [MinLength(1)]
        public List<OrderItemDto>? Items { get; set; }
    }
}
