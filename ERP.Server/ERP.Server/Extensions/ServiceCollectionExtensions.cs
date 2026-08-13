using ERP.Server.Repositories.Implementations;
using ERP.Server.Repositories.Interfaces;
using ERP.Server.Services.Implementations;
using ERP.Server.Services.Interfaces;

namespace ERP.Server.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection Services)
        {
            Services.AddScoped<IUserRepository, UserRepository>();
            Services.AddScoped<ICompanyRepository, CompanyRepository>();
            Services.AddScoped<IGoodsRepository, GoodsRepository>();
            Services.AddScoped<IGoodsCategoryRepository, GoodsCategoryRepository>();
            Services.AddScoped<ISupplierRepository, SupplierRepository>();
            Services.AddScoped<ILocationRepository, LocationRepository>();
            Services.AddScoped<IAreaRepository, AreaRepository>();
            return Services;
        }

        public static IServiceCollection AddServices(this IServiceCollection Services)
        {
            Services.AddScoped<IUserService, UserService>();
            Services.AddScoped<ICompanyService, CompanyService>();
            Services.AddScoped<IGoodsService, GoodsService>();
            Services.AddScoped<IGoodsCategoryService, GoodsCategoryService>();
            Services.AddScoped<ISupplierService, SupplierService>();
            Services.AddScoped<ILocationService, LocationService>();
            Services.AddScoped<IAreaService, AreaService>();
            return Services;
        }
    }
}
