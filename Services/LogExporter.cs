using OfficeOpenXml;
using DicomModifier.Models;

namespace DicomModifier.Services
{
    public static class LogExporter
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
                worksheet.Cells[i + 2, 3].Value = log.EventType;
                worksheet.Cells[i + 2, 4].Value = log.EventSeverity.ToString();
                worksheet.Cells[i + 2, 5].Value = log.Message;
            }

            // AutoFit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Save the file
            FileInfo file = new(filePath);
            package.SaveAs(file);
        }
    }

    public enum EventSeverity
    {
        Informational,
        Warning,
        Error
    }
}