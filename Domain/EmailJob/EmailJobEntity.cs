using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MailCrafter.Domain
{
    public class EmailJobEntity : MongoEntityBase
    {
        public string Name { get; set; }

        public string TemplateId { get; set; }

        public string TemplateName { get; set; }

        public string FromEmail { get; set; }

        public List<string> Recipients { get; set; }

        public bool IsPersonalized { get; set; }

        public string GroupId { get; set; }

        public List<string> CC { get; set; }

        public List<string> BCC { get; set; }

        public Dictionary<string, string> CustomFields { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ScheduledFor { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string ErrorMessage { get; set; }

        public int TotalRecipients { get; set; }

        public int ProcessedRecipients { get; set; }

        public int FailedRecipients { get; set; }

        public EmailJobEntity()
        {
            Recipients = new List<string>();
            CC = new List<string>();
            BCC = new List<string>();
            CustomFields = new Dictionary<string, string>();
            Status = "Pending";
            CreatedAt = DateTime.UtcNow;
        }
    }
} 