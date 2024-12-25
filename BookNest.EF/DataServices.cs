using BookNest.EF.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUWO.EF.Repositories;

namespace BookNest.EF
{
    public static class DataServices
    {
        public static void AddDataServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        }
    }
}
