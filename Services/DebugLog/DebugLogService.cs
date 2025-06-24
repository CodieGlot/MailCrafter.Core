using System;
using System.Collections.Generic;
using System.Linq;

namespace MailCrafter.Services
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
        public string? RequestUrl { get; set; }
        public string? Note { get; set; }
    }

    public interface IDebugLogService
    {
        void AddLog(LogEntry entry);
        List<LogEntry> GetLogs();
    }

    public class DebugLogService : IDebugLogService
    {
        private readonly List<LogEntry> _logs = new List<LogEntry>();
        private readonly int _maxLogEntries = 100;

        public void AddLog(LogEntry entry)
        {
            lock (_logs)
            {
                if (_logs.Count >= _maxLogEntries)
                {
                    _logs.RemoveAt(0);
                }
                _logs.Add(entry);
            }
        }

        public List<LogEntry> GetLogs()
        {
            lock (_logs)
            {
                return _logs.OrderByDescending(l => l.Timestamp).ToList();
            }
        }
    }
} 