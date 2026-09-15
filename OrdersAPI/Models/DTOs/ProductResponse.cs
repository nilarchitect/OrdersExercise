using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersAPI.Models.DTOs
{
    public class ProductResponse
    {        
        public int Id { get; set; }
        public string? Name { get; set; }      
        public decimal Price { get; set; }
    }
}
