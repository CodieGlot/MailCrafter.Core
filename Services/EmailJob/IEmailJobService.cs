using System.Collections.Generic;
using System.Threading.Tasks;
using MailCrafter.Domain;

namespace MailCrafter.Services
{
    public interface IEmailJobService
    {
        Task<EmailJobEntity> CreateAsync(EmailJobEntity job);
        Task<EmailJobEntity> GetByIdAsync(string id);
        Task<List<EmailJobEntity>> GetJobsByUserId(string userId);
        Task<EmailJobEntity> UpdateAsync(EmailJobEntity job);
        Task DeleteAsync(string id);
        Task UpdateStatusAsync(string id, string status, string errorMessage = null);
        Task IncrementProcessedCountAsync(string id, bool isFailed = false);
    }
} 