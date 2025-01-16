using MailCrafter.Services;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.AppUser;
public class AppUserServiceTests : CoreBaseTest
{
    private readonly IAppUserService _appUserService;
    public AppUserServiceTests()
    {
        _appUserService = this.ServiceProvider.GetRequiredService<IAppUserService>();
    }
}
