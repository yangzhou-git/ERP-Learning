using ERP.Server.Repositories.Implementations;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Implementations;
using ERP.Server.Services.Interfaces;

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

            return services;
        }
    }
}