using MailCrafter.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace MailCrafter.Services
{
   
    public class EmailTrackingService : IEmailTrackingService
    {
        private readonly IEmailJobService _emailJobService;
        private readonly ILogger<EmailTrackingService> _logger;
        private readonly string _baseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly string[] ProxyUserAgents = new[]
        {
            "GoogleImageProxy",      // Gmail's Proxy
            "GmailImageProxy",       // Another variant for Gmail
            "YahooMailProxy",        // Yahoo's Proxy
            "YahooCacheSystem",      // Another variant for Yahoo
            "Microsoft-WebDAV-MiniRedir", // Sometimes associated with Outlook/Office link previews
            "camo-proxy"             // Image proxy used by GitHub and others
        };

      
        public EmailTrackingService(IEmailJobService emailJobService, ILogger<EmailTrackingService> logger, IConfiguration configuration)
        {
            _emailJobService = emailJobService;
            _logger = logger;
            _baseUrl = configuration["Tracking:BaseUrl"]!;
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

        private bool IsProxyRequest(string userAgent) // Removed the 'ip' parameter
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return false;
            }

            // Check if the user agent contains any of the known proxy strings.
            return ProxyUserAgents.Any(proxy => userAgent.Contains(proxy, StringComparison.OrdinalIgnoreCase));
        }

        public async Task TrackEmailOpen(string jobId, string recipientEmail)
        {
            try
            {
                var httpContext = _httpContextAccessor?.HttpContext;
                var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? string.Empty;
                // We no longer need the IP for this simple check
                // var ip = httpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

                // Use the corrected proxy check
                if (IsProxyRequest(userAgent))
                {
                    _logger.LogInformation("Open ignored due to proxy User-Agent. UA: {UserAgent}", userAgent);
                    return;
                }

                var job = await _emailJobService.GetByIdAsync(jobId);
                if (job != null)
                {
                    if (job.OpenedRecipientCounts == null)
                        job.OpenedRecipientCounts = new Dictionary<string, int>();

                    var key = recipientEmail.ToLowerInvariant();

                    // Check if this is the first time this recipient is opening the email
                    if (!job.OpenedRecipientCounts.ContainsKey(key))
                    {
                        job.OpenedRecipientCounts[key] = 1;
                        job.OpenedEmails++; // This now tracks UNIQUE opens
                        await _emailJobService.UpdateAsync(job);
                        _logger.LogInformation("Tracked FIRST unique email open for job {JobId}, recipient {Email}", jobId, recipientEmail);
                    }
                    else
                    {
                        // This is a subsequent open by the same recipient.
                        // We can still track it for a "total opens" metric if we want.
                        job.OpenedRecipientCounts[key]++;
                        await _emailJobService.UpdateAsync(job); // Update the total count
                        _logger.LogInformation("Tracked subsequent email open for job {JobId}, recipient {Email} (total opens for recipient: {Count})", jobId, recipientEmail, job.OpenedRecipientCounts[key]);
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