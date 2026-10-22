using API.Context;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string connectionString = builder.Configuration
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        builder.Services.AddDbContext<DataBaseContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services.AddScoped<IUser, UserService>();
        builder.Services.AddScoped<IGame, GameService>();

        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
