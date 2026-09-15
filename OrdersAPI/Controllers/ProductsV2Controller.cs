using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrdersAPI.Services.Contracts;
using OrdersAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace OrdersAPI.Controllers
{
    [ApiController]
    [Route("api/v2/products")]
    [Authorize]
    public class ProductsV2Controller : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        public ProductsV2Controller(IMapper mapper, IProductService productService)
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
            var productResponses = _mapper.Map<IEnumerable<ProductResponseV2>>(products);

            ApiResponse<List<ProductResponseV2>> finalResponse = 
                    new ApiResponse<List<ProductResponseV2>>()
                    {
                        Data = productResponses.ToList(),
                        Message = "Products retrieved successfully"
                    };

            return Ok(finalResponse);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductResponseV2>),statusCode:200)]
        [ProducesResponseType(statusCode:401)]
        [ProducesResponseType(statusCode:404)]
        public IActionResult GetProduct(int id)
        {
            try
            {                
                var product = _productService.GetProduct(id);
                var productResponse = _mapper.Map<ProductResponseV2>(product);

                ApiResponse<ProductResponseV2> finalResponse =
                    new ApiResponse<ProductResponseV2>()
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
