using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersAPI.Models.DTOs
{
    public class ProductResponseV2
    {        
        public int Id { get; set; }
        public string? Name { get; set; }      
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
