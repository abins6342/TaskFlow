
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using User_Service.Db;
using User_Service.DTOs;
using User_Service.Interfaces;
using User_Service.Repositories;
using User_Service.Services;
using UserService.Helper;
using UserService.Infrastructure;
using UserService.Interfaces;
using UserService.Middleware;
using UserService.Services;
using UserService.Validation;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {

            try
            {
                Log.Information("Starting UserService microservice...");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((context,loggerConfig) =>
                {
                    loggerConfig.ReadFrom.Configuration(context.Configuration);
                });

                // Add services to the container.

                builder.Services.AddControllers();
                builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
                builder.Services.AddFluentValidationAutoValidation();

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                // ----------------------------------------------------
                // Configure Database (PostgreSQL)
                // ----------------------------------------------------
                builder.Services.AddDbContext<UserDbContext>(options =>
                                    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

                // ----------------------------------------------------
                // Register Services (Dependency Injection)
                // ----------------------------------------------------

                builder.Services.AddScoped<IUserService, UserServices>();
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
                builder.Services.AddScoped<ITokenService, JwtTokenService>();
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
                // ----------------------------------------------------
                // JWT Authentication Configuration
                // ----------------------------------------------------

                var jwtKey = builder.Configuration["Jwt:Key"];
                var jwtIssuer = builder.Configuration["Jwt:Issuer"];
                var jwtAudience = builder.Configuration["Jwt:Audience"];

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseMiddleware<CorrelationIdMiddleware>();

                app.UseSerilogRequestLogging();
                app.UseMiddleware<ErrorHandlingMiddleware>();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "UserService microservice failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
