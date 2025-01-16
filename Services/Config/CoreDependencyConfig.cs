using MailCrafter.Repositories;
using MailCrafter.Utils.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace MailCrafter.Services;
public static class CoreDependencyConfig
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
    {
        // Register business services
        services.AddScoped<IAppUserService, AppUserService>();

        // Register repositories
        services.AddSingleton<IMongoDBRepository, MongoDBRepository>();
        services.AddScoped<IAppUserRepository, AppUserRepository>();

        // Register other shared services
        services.AddSingleton<IAesEncryptionHelper, AesEncryptionHelper>();

        return services;
    }
}
