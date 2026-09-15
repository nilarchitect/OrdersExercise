using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrdersAPI.Services.Contracts;
using OrdersAPI.Models.DTOs;

namespace OrdersAPI.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        public ProductsController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }
        [HttpGet]
        public IActionResult GetProducts([FromQuery] int pageNumber = 1, 
                                         [FromQuery] int pageSize = 10, 
                                         [FromQuery] decimal? minPrice = null, 
                                         [FromQuery] string? sortBy = null)
        {
            var products = _productService.GetAllProducts(pageNumber, pageSize, minPrice, sortBy);
            var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(products);

            ApiResponse<List<ProductResponse>> finalResponse = 
                    new ApiResponse<List<ProductResponse>>()
                    {
                        Data = productResponses.ToList(),
                        Message = "Products retrieved successfully"
                    };

            return Ok(finalResponse);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            try
            {                
                var product = _productService.GetProduct(id);
                var productResponse = _mapper.Map<ProductResponse>(product);

                ApiResponse<ProductResponse> finalResponse =
                    new ApiResponse<ProductResponse>()
                    {
                        Data = productResponse,
                        Message = "Product retrieved successfully"
                    };

                return Ok(finalResponse);
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (KeyNotFoundException ke)
            {
                return NotFound(ke.Message);
            }
        }

    }
}
