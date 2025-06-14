using apbd12_DBfirstEFCore.Models;
using apbd12_DBfirstEFCore.Services;
using Microsoft.EntityFrameworkCore;

namespace apbd12_DBfirstEFCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        builder.Services.AddDbContext<TripsContext>(opt =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                opt.UseSqlServer(connectionString);
            }
            );

        builder.Services.AddScoped<ITripsService, TripsService>();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();
        app.UseAuthorization();

        
        app.Run();
    }
}