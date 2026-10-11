
using BANK.BLL.Services;
using BANK.DAL.Data;
using BANK.DAL.Entity;
using BANK.DAL.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
            builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<AppDbContext>();

            builder.Services
    .AddAuthentication("Basic")
    .AddScheme<
        AuthenticationSchemeOptions,
        BasicAuthenticationHandler>(
        "Basic",
        null);

            builder.Services.AddAuthorization();

            builder.Services.AddScoped
    <IBankAccountRepository, BankAccountRepository>();

            builder.Services.AddScoped
                <IBankAccountService, BankAccountService>();


            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    Description = "Enter username and password"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Basic"
                }
            },
            Array.Empty<string>()
        }
    });
            });

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


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
