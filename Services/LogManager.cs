using System;
using Microsoft.Data.Sqlite;

namespace DicomModifier.Services
{
    public class LogManager
    {
        private readonly string _connectionString;

        public enum EventType
        {
            UserLoggedIn,
            UserLoginFailed,
            UserCreated,
            UserDeleted,
            PasswordChanged
        }

        public enum EventSeverity
        {
            Informational,
            Warning,
            Error
        }

        public LogManager(string connectionString)
        {
            _connectionString = connectionString;
            EnsureAuditLogTableExists();
        }

        public static void LogActivity(string username, string action, EventSeverity severity)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                        INSERT INTO AuditLog (Timestamp, Username, Action, Severity)
                        VALUES (@timestamp, @username, @action, @severity)";
            command.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@action", action);
            command.Parameters.AddWithValue("@severity", severity.ToString());
            command.ExecuteNonQuery();
        }

        public void EnsureAuditLogTableExists()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='AuditLog'";
            var result = command.ExecuteScalar();
            if (result == null)
            {
                DatabaseHelper.CreateAuditLogTable();
            }
        }
    }
}
