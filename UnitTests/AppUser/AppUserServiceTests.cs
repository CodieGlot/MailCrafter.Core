using MailCrafter.Domain;
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
    public async Task Demo_ShouldRunSuccessfully()
    {
        // Act
        await Demo();
    }
    public async Task Demo()
    {
        // Create new account
        var newAccount = new AppUserEntity
        {
            Email = "toanle.coding@gmail.com",
            Password = "123456789",
            Username = "letoan311AppUser",
            EmailAccounts = new List<EmailAccount>()
        };
        // Test create and get functions
        var result = await _appUserService.Create(newAccount);
        newAccount = await _appUserService.GetByUsernameOrEmail(newAccount.Email);
        newAccount.Password = "123456789@";
        var result2 = await _appUserService.Update(newAccount);
        var result3 = await _appUserService.GetById(newAccount.ID);
        // Create new email account
        var newEmailAccount = new EmailAccount()
        {
            Status = EmailAccountStatus.NotVerified,
            Email = "22521489@gm.uit.edu.vn",
            AppPassword = "1234 5678 9101 1023",
            Alias = "Trưởng phòng nhân sự"
        };
        // Test email account related functions
        var result4 = await _appUserService.AddEmailAccount(newAccount.ID, newEmailAccount.Email, newEmailAccount.AppPassword);
        var result5 = await _appUserService.UpdateEmailPassword(newAccount.ID, "toanhola31912004@gmail.com", newEmailAccount.AppPassword);
        var result6 = await _appUserService.GetByUsernameOrEmail(newAccount.Username);
        var result7 = await _appUserService.GetEmailAccountsOfUser(newAccount.Username);
        var result8 = await _appUserService.RemoveEmailAccount(newAccount.ID, "toanholan31912004@gmail.com");
        var result9 = await _appUserService.Delete(newAccount.ID);
    }
}
