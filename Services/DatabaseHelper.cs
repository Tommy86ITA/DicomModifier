using Microsoft.Data.Sqlite;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SQLitePCL;

namespace DicomModifier.Services
{
    public static class DatabaseHelper
    {
        private static readonly string _databasePath = "UserDatabase.db";
        private static string? _databasePassword;

        static DatabaseHelper()
        {
            // Inizializzare SQLCipher
            SQLitePCL.Batteries_V2.Init();
            raw.SetProvider(new SQLite3Provider_sqlcipher());

            LoadDatabasePassword();
        }

        private static void LoadDatabasePassword()
        {
            try
            {
                var config = JObject.Parse(File.ReadAllText("DBpsw.json"));
                _databasePassword = config["DatabasePassword"]?.ToString();
                if (string.IsNullOrWhiteSpace(_databasePassword))
                {
                    throw new InvalidOperationException("La password del database non può essere null o vuota.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante il caricamento della password del database: {ex.Message}");
                throw;
            }
        }

        public static void InitializeDatabase()
        {
            if (!File.Exists(_databasePath))
            {
                using var connection = GetConnection();
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE Users (
                        Username TEXT PRIMARY KEY,
                        PasswordHash TEXT NOT NULL,
                        Role TEXT NOT NULL,
                        IsEnabled INTEGER NOT NULL
                    )";
                command.ExecuteNonQuery();
                Console.WriteLine("Database creato con successo.");

                // Creazione dell'utente admin
                command.CommandText = @"
                    INSERT INTO Users (Username, PasswordHash, Role, IsEnabled)
                    VALUES ('admin', $passwordHash, 'Administrator', 1)";
                command.Parameters.AddWithValue("$passwordHash", BCrypt.Net.BCrypt.HashPassword("admin123"));
                command.ExecuteNonQuery();
                Console.WriteLine("Utente admin creato con successo.");
            }
            else
            {
                Console.WriteLine("Il database esiste già.");
            }
        }

        public static SqliteConnection GetConnection()
        {
            if (string.IsNullOrWhiteSpace(_databasePassword))
            {
                throw new InvalidOperationException("La password del database non è stata caricata correttamente.");
            }

            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = _databasePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Password = _databasePassword
            }.ToString();

            return new SqliteConnection(connectionString);
        }
    }
}
