using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestaurantManageSystem.Application.Interfaces;
using RestaurantManageSystem.Application.Mappings;
using RestaurantManageSystem.Application.Services;
using RestaurantManageSystem.Domain.Entities;
using RestaurantManageSystem.Infrastructure.Data;
using RestaurantManageSystem.Infrastructure.Repositories;
using RestaurantManageSystem.Infrastructure.Repositories.CategoryRep;
using RestaurantManageSystem.Infrastructure.Repositories.MenuRep;
using RestaurantManageSystem.Infrastructure.Repositories.OrderRep;
using RestaurantManageSystem.Infrastructure.Repositories.TableRep;
using RestaurantManageSystem.Infrastructure.Services;
using ResturantManageSystem.API.Middleware;
using Serilog;
using System.Text;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        "logs/app.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    
    // Register FluentValidation validators
    builder.Services.AddValidatorsFromAssemblyContaining<RestaurantManageSystem.Application.Mappings.MappingProfile>();
    
    // Add Memory Caching
    builder.Services.AddMemoryCache();
    
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "Restaurant Management API", 
            Version = "v1",
            Description = "API for managing restaurant operations including orders, menu items, and tables",
            Contact = new OpenApiContact
            {
                Name = "Support Team",
                Email = "support@restaurant.com"
            }
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddAutoMapper(typeof(MappingProfile));

    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IMenuService, MenuService>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<ITableService, TableService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<ISettingsService, SettingsService>();
    builder.Services.AddScoped<ICacheService, CacheService>();
    builder.Services.AddScoped<IAuditLogService, AuditLogService>();

    // Register custom repositories
    builder.Services.AddScoped(typeof(IRepository<Order>), typeof(OrderRepository));
    builder.Services.AddScoped(typeof(IRepository<MenuItem>), typeof(MenuItemRepository));
    builder.Services.AddScoped(typeof(IRepository<Category>), typeof(CategoryRepository));
    builder.Services.AddScoped(typeof(IRepository<Table>), typeof(TableRepository));

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Add rate limiting middleware
    app.UseMiddleware<RateLimitingMiddleware>();

    // Add exception handling middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Management API");
        });
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}