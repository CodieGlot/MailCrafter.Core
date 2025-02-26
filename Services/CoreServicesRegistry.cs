using MailCrafter.Repositories;
using MailCrafter.Utils.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace MailCrafter.Services;
public static class CoreServicesRegistry
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Register business services
        services.AddSingleton<ITaskQueuePublisher, TaskQueuePublisher>();

        services.AddScoped<IAppUserService, AppUserService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<ICustomGroupService, CustomGroupService>();
        services.AddScoped<IEmailScheduleService, EmailScheduleService>();

        // Register worker services
        services.AddScoped<IEmailSendingService, EmailSendingService>();

        // Register repositories
        services.AddSingleton<IMongoClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            return new MongoClient(configuration["MongoDB:ConnectionURI"]);
        });
        services.AddScoped<IMongoDBRepository, MongoDBRepository>();
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<ICustomGroupRepository, CustomGroupRepository>();
        services.AddScoped<IEmailScheduleRepository, EmailScheduleRepository>();

        // Register other shared services
        services.AddSingleton<IAesEncryptionHelper, AesEncryptionHelper>();

        return services;
    }
}
