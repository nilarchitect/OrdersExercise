using OrdersAPI.Services.Contracts;

namespace OrdersAPI.Adapters
{
    public class PaymentService: IPaymentService
    {
        public async Task<bool> ProcessPayment(decimal ammount)
        {
            await Task.Delay(3000); // Simulate payment processing delay
            return new Random().Next(0, 2) == 1; // Randomly return true or false
        }
    }
}
