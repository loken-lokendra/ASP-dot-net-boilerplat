using ASPDOTNETDEMO.Data;
using ASPDOTNETDEMO.DTOs.Auth;
using ASPDOTNETDEMO.Models;
using ASPDOTNETDEMO.Services;
using ASPDOTNETDEMO.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ASPDOTNETDEMO.Helpers;
using System;

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

        //  REGISTER
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            try
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
            catch (DbUpdateException dbEx)
            {
                // Handle database-specific errors
                return StatusCode(500, ResponseHelper.ServerError<object>($"Database error: {dbEx.Message}"));
            }
            catch (Exception ex)
            {
                // Catch all unexpected errors
                return StatusCode(500, ResponseHelper.ServerError<object>($"An unexpected error occurred: {ex.Message}"));
            }
        }

        //  LOGIN
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ResponseHelper.ServerError<object>($"An unexpected error occurred: {ex.Message}"));
            }
        }

        //  GET PROFILE
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, ResponseHelper.ServerError<object>($"An unexpected error occurred: {ex.Message}"));
            }
        }
    }
}
