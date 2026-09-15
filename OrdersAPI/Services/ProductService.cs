using OrdersAPI.Data;
using OrdersAPI.Models;
using OrdersAPI.Services.Contracts;

namespace OrdersAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly OrderDbContext _context;

        public ProductService(OrderDbContext context)
        {
            _context = context;
        }
        public List<Product> GetAllProducts(int pageNumber, int pageSize, 
                                        decimal? minPrice, string? sortBy)
        {
            var query = _context.Products.AsQueryable();

            //filter
            if (minPrice.HasValue)
                query = query.Where(p => p.Price == minPrice.Value);

            //sort
            if (sortBy != null)
            {
                var sortString = sortBy.ToLower();
                switch (sortString)
                {
                    case "price_asc":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                }
            }

            //pagination
            query = query.Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

            List<Product> products = query.ToList();
            return products;
        }
        public Product GetProduct(int id)
        {
            if (id<0)
                throw new ArgumentException("Id must be greater than 0");

            var product = _context.Products.FirstOrDefault(p => p.Id == id);

            if (product == null)
                throw new KeyNotFoundException($"Product with Id {id} not found");

            return product;
        }
    }
}
