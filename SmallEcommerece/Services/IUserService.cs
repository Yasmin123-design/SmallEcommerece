using SmallEcommerece.Models;

namespace SmallEcommerece.Services
{
    public interface IUserService
    {
        Task<User> Register(User user, string password);
        Task<User> Authenticate(string username, string password);
        Task<User> GetByIdAsync(int id);
    }

}
