using OrdersAPI.Models;
using OrdersAPI.Models.DTOs;

namespace OrdersAPI.Mappings
{
    public class UserMappingProfile : AutoMapper.Profile
    {
        public UserMappingProfile()
        {
            CreateMap<CreateUserRequest, User>();
            
        }
    }
}
