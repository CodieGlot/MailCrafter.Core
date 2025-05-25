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
            TemplateID = "67b1641f204c8d7bf4162658",
            Recipients = new List<string> { "example@fpt.edu.vn" },
            FromMail = "example@gmail.com",
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
            Subject = "Share Your Thoughts: Email Usage Survey 📩",
            Body = @"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; background-color: #f8f9fa; border-radius: 8px;'>
                    <div style='background-color: #007bff; padding: 20px; border-radius: 8px 8px 0 0; text-align: center;'>
                        <h2 style='color: #ffffff; margin: 0;'>We Value Your Input!</h2>
                    </div>
                    <div style='padding: 20px; background-color: #ffffff; border-radius: 0 0 8px 8px;'>
                        <p style='color: #333; font-size: 16px;'>Dear {{RecipientName}},</p>
                        <p style='color: #555; font-size: 14px; line-height: 1.6;'>
                            Your experience matters to us! We are conducting a brief survey to understand how you use emails in your daily workflow. 
                            Your feedback will help us enhance communication tools and improve efficiency.
                        </p>
                        <p style='color: #555; font-size: 14px;'><strong>The survey takes less than 2 minutes!</strong></p>
                        <div style='text-align: center; margin: 20px 0;'>
                            <a href='https://forms.gle/qT8DFpzurGvdhQEu5' 
                               style='background-color: #007bff; color: #ffffff; padding: 12px 24px; font-size: 16px; text-decoration: none; border-radius: 6px; display: inline-block;'>
                               Take the Survey
                            </a>
                        </div>
                        <p style='color: #555; font-size: 14px;'>Thank you for your time and valuable insights!</p>
                        <p style='color: #555; font-size: 14px;'>Best regards,<br><strong>The MailCrafter Team</strong></p>
                    </div>
                </div>
            "
        });
        Assert.NotNull(result.InsertedID);
    }
}
