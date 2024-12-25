using Microsoft.Extensions.DependencyInjection;
using RepositoryWithUOW.Core.AutoMapperProfiles;
using RepositoryWithUOW.Core.Interfaces;
using RepositoryWithUOW.Core.Services;

public static class CoreServices
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthServices, AuthServices>();
        services.AddAutoMapper(typeof(ProfileMapper).Assembly);

    }
}

