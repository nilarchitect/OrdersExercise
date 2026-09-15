namespace OrdersAPI.Services.Contracts
{
    public interface IPaymentService
    {
        public Task<bool> ProcessPayment(decimal amount);
    }
   
}
