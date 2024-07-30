using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace DicomModifier.Services
{
    public class LogExporter
    {
        public static void ExportLogsToExcel(string filePath, List<LogEntry> logs)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Audit Logs");

            // Header row
            worksheet.Cells[1, 1].Value = "Timestamp";
            worksheet.Cells[1, 2].Value = "Username";
            worksheet.Cells[1, 3].Value = "Event";
            worksheet.Cells[1, 4].Value = "Severity";
            worksheet.Cells[1, 5].Value = "Message";

            // Data rows
            for (int i = 0; i < logs.Count; i++)
            {
                var log = logs[i];
                worksheet.Cells[i + 2, 1].Value = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                worksheet.Cells[i + 2, 2].Value = log.Username;
                worksheet.Cells[i + 2, 3].Value = log.Event;
                worksheet.Cells[i + 2, 4].Value = log.Severity.ToString();
                worksheet.Cells[i + 2, 5].Value = log.Message.ToString();
            }

            // AutoFit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Save the file
            FileInfo file = new(filePath);
            package.SaveAs(file);
        }
    }

    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }
        public string EventType { get; set; }
        public EventSeverity EventSeverity { get; set; }
        public Message? Message { get; set; }
    }

    public enum EventSeverity
    {
        Informational,
        Warning,
        Error
    }
}

