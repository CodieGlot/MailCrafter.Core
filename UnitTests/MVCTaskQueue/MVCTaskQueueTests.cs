using MailCrafter.Domain;
using MailCrafter.Services;
using MailCrafter.Utils.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.MVCTaskQueue;
public class MVCTaskQueueTests : CoreBaseTest
{
    [Fact]
    public async Task SendEmailTaskAsync()
    {
        var encryptionHelper = this.ServiceProvider.GetRequiredService<IAesEncryptionHelper>();
        var details = new BasicEmailDetailsModel
        {
            TemplateID = "67b1641f204c8d7bf4162658",
            Recipients = ["example@fpt.edu.vn"],
            FromMail = "example@gmail.com",
            AppPassword = encryptionHelper.Encrypt("app-password"),
        };

        var taskQueue = this.ServiceProvider.GetRequiredService<MVCTaskQueueInstance>();

        await taskQueue.EnqueueAsync(WorkerTaskNames.Send_Basic_Email, details);
    }
}
