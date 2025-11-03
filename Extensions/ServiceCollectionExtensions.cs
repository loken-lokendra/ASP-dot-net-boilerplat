using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ASPDOTNETDEMO.Data;
using ASPDOTNETDEMO.Services;
using ASPDOTNETDEMO.Services.Users;
using ASPDOTNETDEMO.Extensions;

namespace ASPDOTNETDEMO.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 🔹 Controllers
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            // 🔹 Swagger
            services.AddSwaggerWithJwtAuth();

            // 🔹 DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // 🔹 JWT Authentication
            services.AddJwtAuthentication(configuration);

            // 🔹 Authorization
            services.AddAuthorization();

            // 🔹 Custom Services
            services.AddScoped<JwtTokenService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
