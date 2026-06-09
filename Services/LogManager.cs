// Interfaces/LogManager.cs

using DicomModifier.Models;

namespace DicomModifier.Services
{
    public class LogManager()
    {
        //private readonly string _connectionString = connectionString;

        public static void LogActivity(string username, EventMapping.EventType eventType, string? message = null)
        {
            string eventMessage = message ?? eventType.ToString();

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO AuditLog (Username, EventType, Timestamp, Eventseverity, Message) VALUES ($username, $eventType, $timestamp, $eventseverity, $message)";
            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$eventType", eventType.ToString());
            command.Parameters.AddWithValue("$timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("$eventseverity", EventMapping.GetSeverity(eventType).ToString());
            command.Parameters.AddWithValue("$message", eventMessage);
            command.ExecuteNonQuery();
        }
    }
}