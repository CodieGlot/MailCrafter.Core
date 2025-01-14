using MailCrafter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.AppUser;
public class AppUserServiceTests
{
    //private readonly IAppUserService _appUserService;
    public AppUserServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("../../Services/appsettings.Development.json", optional: false, reloadOnChange: true)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddCoreDependencies(configuration)
            .BuildServiceProvider();

        // _appUserService = serviceProvider.GetRequiredService<IAppUserService>();
    }
}
