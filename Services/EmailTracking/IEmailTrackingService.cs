using MailCrafter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailCrafter.Services
{
    public interface IEmailTrackingService 
    {
        string GenerateTrackingPixel(string jobId, string recipientEmail);
        string GenerateTrackingLink(string originalUrl, string jobId, string recipientEmail);
        Task TrackEmailOpen(string jobId, string recipientEmail);
        Task TrackEmailClick(string jobId, string recipientEmail);
    }
}
