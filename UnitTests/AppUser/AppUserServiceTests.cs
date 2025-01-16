using MailCrafter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MailCrafter.Domain;
using MailCrafter.Repositories;
using MailCrafter.Utils.Helpers;

namespace UnitTests.AppUser;
public class AppUserServiceTests
{
    private readonly IAppUserService _appUserService;
    public AppUserServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(@"C:\MailCrafter\Development\Core\appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddCoreDependencies(configuration)
            .AddSingleton<IConfiguration>(configuration)
            .BuildServiceProvider();

        _appUserService = serviceProvider.GetRequiredService<IAppUserService>();
    }
}
