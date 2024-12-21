using Microsoft.Extensions.DependencyInjection;
using RepositoryWithUOW.Core.AutoMapperProfiles;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Services;

public static class ServiceRegistration
{
    public static void RegisterCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthServices, AuthServices>();
        services.AddAutoMapper(typeof(ProfileMapper).Assembly);
    }
}

//internal class UnitOfWork : IUnitOfWork
//{
//    public IBaseRepository<Author> Authors { get; }
//    public IBaseRepository<Book> Books { get; }
//    public Task<int> SaveAsync() => Task.FromResult(0);
//    public void Dispose() { }
//}
