using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace DicomModifier.Services
{
    public static class DatabaseHelper
    {
        private const string _databasePath = "UserDatabase.db";

        public static void InitializeDatabase()
        {
            if (!File.Exists(_databasePath))
            {
                CreateDatabase();
            }
            else
            {
                Console.WriteLine("Database already exists.");
            }

            using var connection = GetConnection();
            connection.Open();

            EnsureTablesExist(connection);
            EnsureAdminUserExists(connection);
        }

        private static void CreateDatabase()
        {
            using var connection = GetConnection();
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                CREATE TABLE Users (
                    Username TEXT PRIMARY KEY,
                    PasswordHash TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    IsEnabled INTEGER NOT NULL
                )";
            command.ExecuteNonQuery();

            command.CommandText = @"
                CREATE TABLE AuditLog (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    Action TEXT NOT NULL,
                    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                )";
            command.ExecuteNonQuery();

            Console.WriteLine("Database and tables created successfully.");
        }

        private static void EnsureTablesExist(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();

            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Users'";
            var result = command.ExecuteScalar();
            if (result == null)
            {
                command.CommandText = @"
                    CREATE TABLE Users (
                        Username TEXT PRIMARY KEY,
                        PasswordHash TEXT NOT NULL,
                        Role TEXT NOT NULL,
                        IsEnabled INTEGER NOT NULL
                    )";
                command.ExecuteNonQuery();
                Console.WriteLine("Table 'Users' created successfully.");
            }

            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='AuditLog'";
            result = command.ExecuteScalar();
            if (result == null)
            {
                command.CommandText = @"
                    CREATE TABLE AuditLog (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL,
                        Action TEXT NOT NULL,
                        Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                    )";
                command.ExecuteNonQuery();
                Console.WriteLine("Table 'AuditLog' created successfully.");
            }
        }

        private static void EnsureAdminUserExists(SqliteConnection connection)
        {
            using var command = connection.CreateCommand();

            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
            long adminCount = (long)(command.ExecuteScalar() ?? 0); // Usa l'operatore null-coalescing

            if (adminCount == 0)
            {
                command.CommandText = @"
                    INSERT INTO Users (Username, PasswordHash, Role, IsEnabled)
                    VALUES ('admin', $passwordHash, 'Administrator', 1)";
                command.Parameters.AddWithValue("$passwordHash", BCrypt.Net.BCrypt.HashPassword("admin123"));
                command.ExecuteNonQuery();
                Console.WriteLine("Admin user created successfully.");
            }
            else
            {
                Console.WriteLine("Admin user already exists.");
            }
        }

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection($"Data Source={_databasePath}");
        }

        public static void LogAudit(string username, string action)
        {
            using var connection = GetConnection();
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO AuditLog (Username, Action)
                VALUES ($username, $action)";
            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$action", action);
            command.ExecuteNonQuery();
        }
    }
}
