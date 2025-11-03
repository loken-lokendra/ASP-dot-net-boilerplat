using ASPDOTNETDEMO.Data;
using ASPDOTNETDEMO.DTOs.Auth;
using ASPDOTNETDEMO.Models;
using ASPDOTNETDEMO.Services;
using ASPDOTNETDEMO.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ASPDOTNETDEMO.Helpers;

namespace ASPDOTNETDEMO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtTokenService _jwtService;
        private readonly IUserService _userService;

        public AuthController(AppDbContext context, JwtTokenService jwtService, IUserService userService)
        {
            _context = context;
            _jwtService = jwtService;
            _userService = userService;
        }

        //  Register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(ResponseHelper.Fail<object>("Email already exists"));

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                RoleId = dto.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(ResponseHelper.Created(user, "User registered successfully"));
        }

        //  Login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(ResponseHelper.Unauthorized<object>("Invalid credentials"));

            var token = _jwtService.GenerateToken(user);

            var responseData = new
            {
                token,
                user = new
                {
                    user.Username,
                    user.Email,
                    Role = user.Role?.Name
                }
            };

            return Ok(ResponseHelper.Success(responseData, "Login successful"));
        }

        //  Get User Profile (Authenticated)
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userService.GetUserProfileAsync(User);

            if (user == null)
                return NotFound(ResponseHelper.NotFound<object>("User not found"));

            var userProfile = new
            {
                user.Username,
                user.Email,
                Role = user.Role?.Name
            };

            return Ok(ResponseHelper.Success(userProfile, "Profile fetched successfully"));
        }
    }
}
