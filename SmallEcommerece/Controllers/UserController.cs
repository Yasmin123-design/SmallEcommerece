using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmallEcommerece.Dtos;
using SmallEcommerece.Models;
using SmallEcommerece.Services;
using SmallEcommerece.UnitOfWorks;

namespace SmallEcommerece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUserService userService, JwtService jwtService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new User
            {
                Username = dto.UserName,
                Email = dto.Email
            };

            var created = await _userService.Register(user, dto.Password);
            return Ok(new { message = "User registered successfully", created.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userService.Authenticate(dto.Email, dto.Password);
            if (user == null) return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.CommitAsync();

            return Ok(new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(5)
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var user = users.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized("Invalid refresh token");

            var newJwt = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.CommitAsync();

            return Ok(new AuthResponseDto
            {
                Token = newJwt,
                RefreshToken = newRefreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(5)
            });
        }
    }
}
