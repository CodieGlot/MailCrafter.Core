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
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<ICustomGroupService, CustomGroupService>();

        // Register worker services
        services.AddScoped<IEmailSendingService, EmailSendingService>();

        // Register repositories
        services.AddSingleton<IMongoDBRepository, MongoDBRepository>();
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<ICustomGroupRepository, CustomGroupRepository>();

        // Register other shared services
        services.AddSingleton<IAesEncryptionHelper, AesEncryptionHelper>();

        return services;
    }
}
