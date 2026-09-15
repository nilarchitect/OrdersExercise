using OrdersAPI.Models;

namespace OrdersAPI.Services.Contracts
{
    public interface IOrderService
    {
        public List<Order> GetOrders();
        public Order GetOrderById(int id);
        public Task<Order> CreateOrder(CreateOrderRequest orderRequest);
       
    }
}
