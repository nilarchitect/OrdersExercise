using System.ComponentModel.DataAnnotations;

namespace OrdersAPI.Models.DTOs
{
    public class CreateUserRequest
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }        
        public string Role { get; set; } = "User"; 
    }
}
