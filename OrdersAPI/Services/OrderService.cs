using Microsoft.Extensions.Caching.Memory;
using OrdersAPI.Data;
using OrdersAPI.Models;
using OrdersAPI.Services.Contracts; 

namespace OrdersAPI.Services;

public class OrderService: IOrderService
{
    private readonly OrderDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly IPaymentService _paymentService;

    public OrderService(OrderDbContext dbContext, IMemoryCache cache, IPaymentService paymentService) 
    {
         _dbContext = dbContext;
         _cache = cache;
         _paymentService = paymentService;
    }

    public List<Order> GetOrders()
    {
        return _dbContext.Orders.ToList();
    }
    public Order GetOrderById(int id)
    {
        if (id < 0)
            throw new ArgumentException("Invalid order Id");

        var order = _dbContext.Orders.FirstOrDefault(order => order.Id == id);
        
        if (order==null)
            throw new ArgumentException($"Order with Id {id} not found");

        return order;
    }
    public async Task<Order> CreateOrder(CreateOrderRequest orderRequest)
    {
        if (orderRequest.Items.Count>10)
            throw new ArgumentException("Order cannot have more than 10 items");

        Console.WriteLine("Creating order with {0} items", orderRequest.Items.Count);

        decimal totalPrice = 0;
        var orderItems = new List<OrderItem>();

        var productIds = orderRequest.Items
            .Select(i => i.ProductId)
            .ToList();      

        if(_cache.TryGetValue("products", out List<Product> products)==false)
        {
            products = _dbContext.Products
                       .Where(p => productIds.Contains(p.Id))
                       .ToList();
            _cache.Set("products", products, TimeSpan.FromMinutes(5));
        }
        var productMap = products.ToDictionary(p => p.Id, p => p);


        foreach (var item in orderRequest.Items)
        {
            if(item.Quantity<=0)
                throw new ArgumentException("Quantity must be greater than 0");

            //var product = _dbContext.Products.FirstOrDefault(p => p.Id == item.ProductId);

            if (!productMap.TryGetValue(item.ProductId, out var product))
                throw new ArgumentException($"Invalid product Id {item.ProductId}");

            totalPrice += product.Price * item.Quantity;

            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };

            orderItems.Add(orderItem);
        }

        var order = new Order
        {          
            Items = orderItems,
            TotalPrice= totalPrice
        };
        int maxRetries = 3;

        var paymentResult = false;

        while (maxRetries > 0)
        {
            var paymentTask = _paymentService.ProcessPayment(totalPrice);          

            if (await Task.WhenAny(paymentTask, Task.Delay(1000)) == paymentTask)
                paymentResult = paymentTask.Result;

            if (paymentResult)
                break;

            maxRetries--;
        }       

        if (!paymentResult)
            throw new Exception("Payment failed");

        _dbContext.Add(order);  
        _dbContext.SaveChanges();

        return order;
    }
}
