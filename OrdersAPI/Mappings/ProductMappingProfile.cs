using OrdersAPI.Models;
using OrdersAPI.Models.DTOs;

namespace OrdersAPI.Mappings
{
    public class ProductMappingProfile: AutoMapper.Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductResponse>();

            CreateMap<Product, ProductResponseV2>();
        }
    }
}
