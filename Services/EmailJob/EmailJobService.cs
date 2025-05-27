using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailCrafter.Domain;
using MailCrafter.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MailCrafter.Services
{
    public class EmailJobService : IEmailJobService
    {
        private readonly IEmailJobRepository _emailJobRepository;
        private readonly ILogger<EmailJobService> _logger;
        private readonly IAppUserService _appUserService;

        public EmailJobService(
            IEmailJobRepository emailJobRepository, 
            ILogger<EmailJobService> logger,
            IAppUserService appUserService)
        {
            _emailJobRepository = emailJobRepository;
            _logger = logger;
            _appUserService = appUserService;
        }

        public async Task<EmailJobEntity> CreateAsync(EmailJobEntity job)
        {
            var result = await _emailJobRepository.CreateAsync(job);
            return job;
        }

        public async Task<EmailJobEntity> GetByIdAsync(string id)
        {
            return await _emailJobRepository.GetByIdAsync(id);
        }

        public async Task<List<EmailJobEntity>> GetJobsByUserId(string userId)
        {
            // Get the user's email accounts
            var emailAccounts = await _appUserService.GetEmailAccountsOfUser(userId);
            if (!emailAccounts.Any())
            {
                return new List<EmailJobEntity>();
            }

            // Get all jobs where FromEmail matches any of the user's email accounts
            var jobs = await _emailJobRepository.FindAsync(j => emailAccounts.Contains(j.FromEmail));
            return jobs;
        }

        public async Task<EmailJobEntity> UpdateAsync(EmailJobEntity job)
        {
            await _emailJobRepository.ReplaceAsync(job.ID, job);
            return job;
        }

        public async Task DeleteAsync(string id)
        {
            await _emailJobRepository.DeleteAsync(id);
        }

        public async Task UpdateStatusAsync(string id, string status, string errorMessage = null)
        {
            var job = await _emailJobRepository.GetByIdAsync(id);
            if (job == null) return;

            job.Status = status;
            if (errorMessage != null)
            {
                job.ErrorMessage = errorMessage;
            }

            if (status == "Processing")
            {
                job.StartedAt = DateTime.UtcNow;
            }
            else if (status == "Completed" || status == "Failed")
            {
                job.CompletedAt = DateTime.UtcNow;
            }

            await _emailJobRepository.ReplaceAsync(id, job);
        }

        public async Task IncrementProcessedCountAsync(string id, bool isFailed = false)
        {
            var job = await _emailJobRepository.GetByIdAsync(id);
            if (job == null) return;

            job.ProcessedRecipients++;
            if (isFailed)
            {
                job.FailedRecipients++;
            }

            await _emailJobRepository.ReplaceAsync(id, job);
        }
    }
} 