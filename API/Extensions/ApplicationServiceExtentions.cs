using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
            IConfiguration config)
        {
            // DefaultConnection should be added in appsettings.json file
            // Program file will pick it up from appsettings.json file
            services.AddDbContext<DataContext>(p =>
            {
                p.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            services.AddCors();

            //builder.Services.AddScoped<TokenService>(); --this can be used but it is good practice to have interface
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IUserRepository, UserRepository>();

            // for injecting automapper into controller 
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}
