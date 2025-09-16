using SmallEcommerece.Models;
using SmallEcommerece.UnitOfWorks;

namespace SmallEcommerece.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> Register(User user, string password)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CommitAsync();
            return user;
        }

        public async Task<User> Authenticate(string useremail, string password)
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == useremail);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            user.LastLoginTime = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CommitAsync();
            return user;
        }

        public async Task<User> GetByIdAsync(int id) =>
            await _unitOfWork.Users.GetByIdAsync(id);
    }

}
