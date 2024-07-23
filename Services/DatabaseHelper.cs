using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace DicomModifier.Services
{
    public static class DatabaseHelper
    {
        private static readonly string _databasePath = "UserDatabase.db";

        public static void InitializeDatabase()
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();

            // Se il database non esiste, crearlo
            if (!File.Exists(_databasePath))
            {
                command.CommandText = @"
                    CREATE TABLE Users (
                        Username TEXT PRIMARY KEY,
                        PasswordHash TEXT NOT NULL,
                        Role TEXT NOT NULL,
                        IsEnabled INTEGER NOT NULL
                    )";
                command.ExecuteNonQuery();
                Console.WriteLine("Database created successfully.");
            }
            else
            {
                Console.WriteLine("Database already exists.");
            }

            // Verifica se l'utente admin esiste
            command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
            long adminCount = (long)(command.ExecuteScalar() ?? 0); // Usa l'operatore null-coalescing

            // Se l'utente admin non esiste, crearlo
            if (adminCount == 0)
            {
                command.CommandText = @"
                    INSERT INTO Users (Username, PasswordHash, Role, IsEnabled)
                    VALUES ('admin', $passwordHash, 'Admin', 1)";
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
    }
}
