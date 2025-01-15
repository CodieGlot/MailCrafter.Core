using MailCrafter.Repositories;
using MailCrafter.Utils.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MailCrafter.Services;
public static class CoreDependencyConfig
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Load settings into options
        services.AddSingleton<IConfiguration>(configuration);

        // Register business services
        services.AddSingleton<IAppUserService, AppUserService>();

        // Register repositories
        services.AddSingleton<IMongoDBRepository, MongoDBRepository>();
        services.AddSingleton<IAppUserRepository, AppUserRepository>();

        // Register other shared services
        services.AddSingleton<IAesEncryptionHelper, AesEncryptionHelper>();

        return services;
    }
}
