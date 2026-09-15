using OrdersAPI.Models;

namespace OrdersAPI.Services.Contracts
{
    public interface IProductService
    {
        public List<Product> GetAllProducts(int pageNumber, int pageSize, 
                                            decimal? minPrice, string? sortBy);
        public Product GetProduct(int id);
    }
}
