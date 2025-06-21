using MailCrafter.Domain;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace MailCrafter.Services
{
    public interface IEmailTrackingService
    {
        string GenerateTrackingPixel(string jobId, string recipientEmail);
        string GenerateTrackingLink(string originalUrl, string jobId, string recipientEmail);
        Task TrackEmailOpen(string jobId, string recipientEmail);
        Task TrackEmailClick(string jobId, string recipientEmail);
    }

    public class EmailTrackingService : IEmailTrackingService
    {
        private readonly IEmailJobService _emailJobService;
        private readonly ILogger<EmailTrackingService> _logger;

        public EmailTrackingService(IEmailJobService emailJobService, ILogger<EmailTrackingService> logger)
        {
            _emailJobService = emailJobService;
            _logger = logger;
        }

        public string GenerateTrackingPixel(string jobId, string recipientEmail)
        {
            var trackingData = $"{jobId}:{recipientEmail}";
            var encryptedData = EncryptTrackingData(trackingData);
            return $"https://localhost:7177/api/tracking/pixel/{encryptedData}";
        }

        public string GenerateTrackingLink(string originalUrl, string jobId, string recipientEmail)
        {
            var trackingData = $"{jobId}:{recipientEmail}";
            var encryptedData = EncryptTrackingData(trackingData);
            return $"https://localhost:7177/api/tracking/click/{encryptedData}?url={Uri.EscapeDataString(originalUrl)}";
        }

        public async Task TrackEmailOpen(string jobId, string recipientEmail)
        {
            try
            {
                var job = await _emailJobService.GetByIdAsync(jobId);
                if (job != null)
                {
                    job.OpenedEmails++;
                    await _emailJobService.UpdateAsync(job);
                    _logger.LogInformation("Tracked email open for job {JobId}, recipient {Email}", jobId, recipientEmail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking email open for job {JobId}, recipient {Email}", jobId, recipientEmail);
            }
        }

        public async Task TrackEmailClick(string jobId, string recipientEmail)
        {
            try
            {
                var job = await _emailJobService.GetByIdAsync(jobId);
                if (job != null)
                {
                    job.ClickedEmails++;
                    await _emailJobService.UpdateAsync(job);
                    _logger.LogInformation("Tracked email click for job {JobId}, recipient {Email}", jobId, recipientEmail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking email click for job {JobId}, recipient {Email}", jobId, recipientEmail);
            }
        }

        private string EncryptTrackingData(string data)
        {
            // In a production environment, use proper encryption
            // This is a simple example using Base64 encoding
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }
    }
} 