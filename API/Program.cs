using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using API.Extensions;

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

app.UseCors(opt =>
    opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
