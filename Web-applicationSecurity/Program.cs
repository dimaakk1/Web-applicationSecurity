
using BANK.BLL.Services;
using BANK.DAL.Data;
using BANK.DAL.Repository;
using Microsoft.EntityFrameworkCore;
using System;

namespace Web_applicationSecurity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
             builder.Configuration.GetConnectionString("DefaultConnection")
            ));

            builder.Services.AddScoped
    <IBankAccountRepository, BankAccountRepository>();

            builder.Services.AddScoped
                <IBankAccountService, BankAccountService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
