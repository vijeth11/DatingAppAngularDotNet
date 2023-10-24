using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
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

            services.AddScoped<ILikesRepository, LikesRepository>();

            // for injecting automapper into controller 
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //configuration of firebase storage used in PhotoService
            services.Configure<FirebaseStorageSettings>(config.GetSection("FirebaseStorageSettings"));

            services.AddScoped<IPhotoService, PhotoService>();

            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<LogUserActivity>();

            services.AddSignalR();

            services.AddSingleton<PresenceTracker>();
            return services;
        }
    }
}
