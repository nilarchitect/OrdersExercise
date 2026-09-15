using OrdersAPI.Models;

namespace OrdersAPI.Services.Contracts
{
    public interface IUserService
    {
        public User CreateUser(User user);
        public List<User> GetUsers();
        public User GetUser(int id);
        public User AuthenticateUser(string username, string password);

    }
}
