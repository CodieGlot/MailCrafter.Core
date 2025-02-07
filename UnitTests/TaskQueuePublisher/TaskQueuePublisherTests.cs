using MailCrafter.Domain;
using MailCrafter.Services;
using MailCrafter.Utils.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.Worker;
public class TaskQueuePublisherTests : CoreBaseTest
{
    public TaskQueuePublisherTests()
    {

    }
    [Fact]
    public async Task PushSendEmailTask()
    {
        var encryptionHelper = this.ServiceProvider.GetRequiredService<IAesEncryptionHelper>();
        var details = new BasicEmailDetailsModel
        {
            TemplateID = "67a4ce14f156db8019d158e2",
            Recipients = new List<string> { "recipient@email.com" },
            FromMail = "from@email.com",
            AppPassword = encryptionHelper.Encrypt("app-password"),
        };
        var publisher = this.ServiceProvider.GetRequiredService<ITaskQueuePublisher>();

        await publisher.PublishMessageAsync(WorkerTaskNames.Send_Basic_Email, details);
    }
    [Fact]
    public async Task Demo()
    {
        var emailTemplateService = this.ServiceProvider.GetRequiredService<IEmailTemplateService>();
        var result = await emailTemplateService.Create(new EmailTemplateEntity
        {
            Subject = "Hello",
            Body = "codie here"
        });
        Assert.NotNull(result.InsertedID);
    }
}
