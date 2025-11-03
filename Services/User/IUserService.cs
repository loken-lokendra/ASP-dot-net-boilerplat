using ASPDOTNETDEMO.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASPDOTNETDEMO.Services.Users
{
    public interface IUserService
    {
        Task<User?> GetUserProfileAsync(ClaimsPrincipal user);
    }
}
