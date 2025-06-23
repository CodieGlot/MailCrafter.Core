using MailCrafter.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Linq;

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
        private readonly string _baseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly string[] ProxyUserAgents = new[]
        {
            "GoogleImageProxy",
            "Google",
            "Outlook",
            "Microsoft Office",
            "YahooCacheSystem",
            "Thunderbird",
            "AppleWebKit"
        };


        public EmailTrackingService(IEmailJobService emailJobService, ILogger<EmailTrackingService> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _emailJobService = emailJobService;
            _logger = logger;
            _baseUrl = configuration["Tracking:BaseUrl"]!;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateTrackingPixel(string jobId, string recipientEmail)
        {
            var trackingData = $"{jobId}:{recipientEmail}";
            var encryptedData = EncryptTrackingData(trackingData);
            //return $"https://localhost:7177/api/tracking/pixel/{encryptedData}";
            return $"https://mailcrafter-f6cyggezagarg5hs.southeastasia-01.azurewebsites.net/api/tracking/pixel/{encryptedData}";
        }

        public string GenerateTrackingLink(string originalUrl, string jobId, string recipientEmail)
        {
            var trackingData = $"{jobId}:{recipientEmail}";
            var encryptedData = EncryptTrackingData(trackingData);
            //return $"https://localhost:7177/api/tracking/click/{encryptedData}?url={Uri.EscapeDataString(originalUrl)}";
            return $"https://mailcrafter-f6cyggezagarg5hs.southeastasia-01.azurewebsites.net/api/tracking/click/{encryptedData}?url={Uri.EscapeDataString(originalUrl)}";
        }

        private bool IsProxyRequest(string userAgent, string ip)
        {
            if (!string.IsNullOrEmpty(userAgent) && ProxyUserAgents.Any(proxy => userAgent.Contains(proxy, StringComparison.OrdinalIgnoreCase)))
                return true;
            if (!string.IsNullOrEmpty(ip))
                return true;
            return false;
        }

        public async Task TrackEmailOpen(string jobId, string recipientEmail)
        {
            try
            {
                var httpContext = _httpContextAccessor?.HttpContext;
                var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? string.Empty;
                var ip = httpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

                if (IsProxyRequest(userAgent, ip))
                {
                    _logger.LogInformation("Open ignored due to proxy User-Agent or IP. UA: {UserAgent}, IP: {IP}", userAgent, ip);
                    return;
                }

                var job = await _emailJobService.GetByIdAsync(jobId);
                if (job != null)
                {
                    if (job.OpenedRecipientCounts == null)
                        job.OpenedRecipientCounts = new Dictionary<string, int>();

                    var key = recipientEmail.ToLowerInvariant();
                    if (!job.OpenedRecipientCounts.ContainsKey(key))
                        job.OpenedRecipientCounts[key] = 0;

                    job.OpenedRecipientCounts[key]++;

                    if (job.OpenedRecipientCounts[key] == 2)
                    {
                        job.OpenedEmails++;
                        await _emailJobService.UpdateAsync(job);
                        _logger.LogInformation("Tracked SECOND email open for job {JobId}, recipient {Email}", jobId, recipientEmail);
                    }
                    else
                    {
                        _logger.LogInformation("Email open for job {JobId}, recipient {Email} (count: {Count})", jobId, recipientEmail, job.OpenedRecipientCounts[key]);
                    }
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
                    if (job.ClickedRecipients == null)
                        job.ClickedRecipients = new HashSet<string>();

                    if (job.ClickedRecipients.Add(recipientEmail.ToLowerInvariant()))
                    {
                        job.ClickedEmails++;
                        await _emailJobService.UpdateAsync(job);
                        _logger.LogInformation("Tracked FIRST email click for job {JobId}, recipient {Email}", jobId, recipientEmail);
                    }
                    else
                    {
                        _logger.LogInformation("Email click already tracked for job {JobId}, recipient {Email}", jobId, recipientEmail);
                    }
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