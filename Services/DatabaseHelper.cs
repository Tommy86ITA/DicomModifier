// Interfaces/DatabaseHelper.cs

using Microsoft.Data.Sqlite;

namespace DicomModifier.Services
{
    public class DatabaseHelper
    {
        //private static readonly string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DicomImport", "UserDatabase.db");
        private const string _databasePath = "UserDatabase.db"; // Path to the SQLite database file

        // Initializes the database by creating it if it doesn't exist and ensuring necessary tables and admin user are present
        public static void InitializeDatabase()
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
        public static void CreateAuditLogTable()
        {  
            using var connection = GetConnection();
            connection.Open(); // Open the connection
            using var command = connection.CreateCommand(); // Create a command to execute SQL queries

            command.CommandText = @"
                CREATE TABLE AuditLog (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    Action TEXT NOT NULL,
                    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Severity TEXT NOT NULL
                )";
            command.ExecuteNonQuery(); // Execute the command
            LogManager.LogActivity("System", "Creato log di audit", LogManager.EventSeverity.Informational);
        }

        // Ensures the required tables exist in the database
        public static void EnsureTablesExist(SqliteConnection connection)
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
        private static void EnsureAdminUserExists(SqliteConnection connection)
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
                LogManager.LogActivity("System", "Creato utente di default",LogManager.EventSeverity.Informational); // Log the admin user creation activity
            }
        }

        // Gets a connection to the SQLite database
        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={_databasePath}"); // Return a new SQLite connection
        }
    }
}
