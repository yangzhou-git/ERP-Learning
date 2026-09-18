using System.Text;
using ERP.Server.Models.Entity;
using ERP.Server.Repositories.Implementations;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Implementations;
using ERP.Server.Services.Implementations.Auth;
using ERP.Server.Services.Interfaces;
using ERP.Server.Services.Interfaces.Arrival;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace ERP.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IGoodsRepository, GoodsRepository>();
            services.AddScoped<IGoodsCategoryRepository, GoodsCategoryRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IGoodsUnitRepository, GoodsUnitRepository>();
            services.AddScoped<IArrivalRepository, ArrivalRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IGoodsService, GoodsService>();
            services.AddScoped<IGoodsCategoryService, GoodsCategoryService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IGoodsUnitService, GoodsUnitService>();
            services.AddScoped<IArrivalService, ArrivalService>();
            services.AddScoped<IInventoryService, InventoryService>();

            return services;
        }

        public static IServiceCollection AddRedis(this IServiceCollection services,IConfiguration configuration)
        {
            var connectionString = configuration["Redis:ConnectionString"]
                ?? throw new InvalidOperationException("Redis连接地址未配置");
            var password = configuration["Redis:Password"]
                ?? throw new InvalidOperationException("Redis密码未配置");
            var redisOptions = new ConfigurationOptions
            {
                Password = password
            };
            redisOptions.EndPoints.Add(connectionString);
            var redis = ConnectionMultiplexer.Connect(redisOptions);
            services.AddSingleton<IConnectionMultiplexer>(redis);
            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,IConfiguration configuration)
        {
            var jwtKey = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("未配置 Jwt:Key");
            var jwtIssuer = configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("未配置 Jwt:Issuer");
            var jwtAudience = configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("未配置 Jwt:Audience");
            services.AddAuthentication(options =>
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
            services.AddAuthorization();
            return services;
        }
    }
}