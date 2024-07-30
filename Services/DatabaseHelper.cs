// Interfaces/DatabaseHelper.cs

using DicomModifier.Models;
using Microsoft.Data.Sqlite;

namespace DicomModifier.Services
{
    public class DatabaseHelper
    {
        //private static readonly string _databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DicomImport", "UserDatabase.db");
        //private static readonly string _connectionString = $"Data Source={_databasePath}";
        private const string _databasePath = "DCMImp.db";
        //private static readonly string _connectionString = $"Data Source={_databasePath}";

        public DatabaseHelper()
        {
            InitializeDatabase();
        }

        // Initializes the database by creating it if it doesn't exist and ensuring necessary tables and admin user are present
        public void InitializeDatabase()
        {
            if (!File.Exists(_databasePath)) // Create a new database if it doesn't exist
            {
                CreateUsersTable();
                CreateAuditLogTable();
            }

            using var connection = GetConnection(); // Get a connection to the database
            connection.Open(); // Open the connection

            EnsureTablesExist(connection); // Ensure the required tables exist in the database
            EnsureAdminUserExists(connection); // Ensure the admin user exists in the Users table
        }

        // Creates the database and the necessary tables
        private static void CreateUsersTable()
        {
            using var connection = GetConnection(); // Get a connection to the database
            connection.Open(); // Open the connection
            using var command = connection.CreateCommand(); // Create a command to execute SQL queries

            // Create the Users table
            command.CommandText = @"
                CREATE TABLE Users (
                    Username TEXT PRIMARY KEY,
                    PasswordHash TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    IsEnabled INTEGER NOT NULL
                )";
            command.ExecuteNonQuery(); // Execute the command
        }

        // Create the AuditLog table
        public void CreateAuditLogTable()
        {
            using var connection = GetConnection();
            connection.Open(); // Open the connection
            using var command = connection.CreateCommand(); // Create a command to execute SQL queries

            command.CommandText = @"
                CREATE TABLE AuditLog (
                    Timestamp TEXT NOT NULL,
                    Username TEXT NOT NULL,
                    EventType TEXT NOT NULL,
                    EventSeverity TEXT NOT NULL,
                    Message TEXT NULLABLE
                )";
            command.ExecuteNonQuery(); // Execute the command
            LogAudit("System", EventMapping.EventType.AuditDBCreated);
        }

        // Ensures the required tables exist in the database
        public void EnsureTablesExist(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();

            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Users'";
            var result = command.ExecuteScalar();
            if (result == null)
            {
                CreateUsersTable();
            }

            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='AuditLog'";
            result = command.ExecuteScalar();
            if (result == null)
            {
                CreateAuditLogTable();
            }
        }

        // Ensures the admin user exists in the Users table
        private void EnsureAdminUserExists(SqliteConnection connection)
        {
            using var command = connection.CreateCommand(); // Create a command to execute SQL queries

            // Check if the admin user exists
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
            long adminCount = (long)(command.ExecuteScalar() ?? 0); // Execute the command and get the result

            if (adminCount == 0)
            {
                // Insert the admin user if it doesn't exist
                command.CommandText = @"
                    INSERT INTO Users (Username, PasswordHash, Role, IsEnabled)
                    VALUES ('admin', $passwordHash, 'Administrator', 1)";
                command.Parameters.AddWithValue("$passwordHash", BCrypt.Net.BCrypt.HashPassword("admin123")); // Hash the default admin password
                command.ExecuteNonQuery(); // Execute the command
                Console.WriteLine("Admin user created successfully."); // Log the successful creation of the admin user
                LogAudit("System", EventMapping.EventType.DefaultAdminCreated); // Log the admin user creation activity
            }
        }

        public void LogAudit(string username, EventMapping.EventType eventType, string? message=null)
        {
            LogManager.LogActivity(username, eventType, message);
        }

        // Gets a connection to the SQLite database
        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={_databasePath}"); // Return a new SQLite connection
        }
    }
}
