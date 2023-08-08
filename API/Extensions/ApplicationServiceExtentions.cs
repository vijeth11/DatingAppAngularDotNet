using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
            //configuration of firebase storage used in PhotoService
            services.Configure<FirebaseStorageSettings>(config.GetSection("FirebaseStorageSettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            return services;
        }
    }
}
