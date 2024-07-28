// Interfaces/DatabaseHelper.cs

using Microsoft.Data.Sqlite;

namespace DicomModifier.Services
{
    public static class DatabaseHelper
    {
        private const string _databasePath = "UserDatabase.db"; // Path to the SQLite database file

        // Initializes the database by creating it if it doesn't exist and ensuring necessary tables and admin user are present
        public static void InitializeDatabase()
        {
            if (!File.Exists(_databasePath))
            {
                CreateDatabase(); // Create a new database if it doesn't exist
            }
            else
            {
                Console.WriteLine("Database already exists."); // Log that the database already exists
            }

            using var connection = GetConnection(); // Get a connection to the database
            connection.Open(); // Open the connection

            EnsureTablesExist(connection); // Ensure the required tables exist in the database
            EnsureAdminUserExists(connection); // Ensure the admin user exists in the Users table
        }

        // Creates the database and the necessary tables
        private static void CreateDatabase()
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

            // Create the AuditLog table
            command.CommandText = @"
                CREATE TABLE AuditLog (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    Action TEXT NOT NULL,
                    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                )";
            command.ExecuteNonQuery(); // Execute the command

            Console.WriteLine("Database and tables created successfully."); // Log the successful creation of the database and tables
        }

        // Ensures the required tables exist in the database
        private static void EnsureTablesExist(SqliteConnection connection)
        {
            using var command = connection.CreateCommand(); // Create a command to execute SQL queries

            // Check if the Users table exists
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Users'";
            var result = command.ExecuteScalar(); // Execute the command and get the result
            if (result == null)
            {
                // Create the Users table if it doesn't exist
                command.CommandText = @"
                    CREATE TABLE Users (
                        Username TEXT PRIMARY KEY,
                        PasswordHash TEXT NOT NULL,
                        Role TEXT NOT NULL,
                        IsEnabled INTEGER NOT NULL
                    )";
                command.ExecuteNonQuery(); // Execute the command
                Console.WriteLine("Table 'Users' created successfully."); // Log the successful creation of the Users table
            }

            // Check if the AuditLog table exists
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='AuditLog'";
            result = command.ExecuteScalar(); // Execute the command and get the result
            if (result == null)
            {
                // Create the AuditLog table if it doesn't exist
                command.CommandText = @"
                    CREATE TABLE AuditLog (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL,
                        Action TEXT NOT NULL,
                        Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";
                command.ExecuteNonQuery(); // Execute the command
                Console.WriteLine("Table 'AuditLog' created successfully."); // Log the successful creation of the AuditLog table
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
            }
            else
            {
                Console.WriteLine("Admin user already exists."); // Log that the admin user already exists
            }
        }

        // Gets a connection to the SQLite database
        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={_databasePath}"); // Return a new SQLite connection
        }

        // Logs an action to the AuditLog table
        public static void LogAudit(string username, string action)
        {
            using var connection = GetConnection(); // Get a connection to the database
            connection.Open(); // Open the connection

            using var command = connection.CreateCommand(); // Create a command to execute SQL queries
            command.CommandText = @"
                INSERT INTO AuditLog (Username, Action)
                VALUES ($username, $action)";
            command.Parameters.AddWithValue("$username", username); // Add the username parameter
            command.Parameters.AddWithValue("$action", action); // Add the action parameter
            command.ExecuteNonQuery(); // Execute the command
        }
    }
}
