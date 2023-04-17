using API.Extensions;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Extension methods defined in Api.Extensions folder
// to make program.cs file look good
builder.Services.AddApplicationServices(builder.Configuration);

// Extension methods defined in Api.Extensions folder
// to make program.cs file look good
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

if(builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(opt =>
    opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
