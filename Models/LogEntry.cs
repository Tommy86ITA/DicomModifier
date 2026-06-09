using DicomModifier.Services;
using DicomModifier.Models;

namespace DicomModifier.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string? Username { get; set; }
        public EventMapping.EventType EventType { get; set; }
        public EventMapping.EventSeverity EventSeverity { get; set; }
        public string? Message { get; set; }
    }
}
