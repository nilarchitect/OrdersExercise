using OrdersAPI.Models;
using OrdersAPI.Services.Contracts;
using OrdersAPI.Data;

namespace OrdersAPI.Services
{
    public class UserService: IUserService
    {
        private readonly OrderDbContext _context;

        public UserService(OrderDbContext context)
        {
            _context = context;
        }

        public User CreateUser(User user)
        {
            var hasshedPassword=BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.PasswordHash = hasshedPassword;
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
        public List<User> GetUsers()
        {
            var users = _context.Users.ToList();
            return users;
        }
        public User GetUser(int id)
        {
            if (id<=0)
                throw new ArgumentException("Invalid user ID");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user==null)
                throw new NullReferenceException("User not found");

            return user;
        }
        public User AuthenticateUser(string username, string password)
        {
            if (username == null || password == null)
                throw new NullReferenceException("User or password is null");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
                throw new NullReferenceException("Invalid username");

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new ArgumentException("Invalid password");

            return user;
        }
    }
}
