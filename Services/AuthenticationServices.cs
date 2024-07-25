using DicomModifier.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DicomModifier.Services
{
    public class AuthenticationService
    {
        public User CurrentUser { get; private set; }

        public AuthenticationService()
        {
            DatabaseHelper.InitializeDatabase();
            CurrentUser = new User(); // Inizializzazione predefinita
        }

        public bool Authenticate(string username, string password)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT PasswordHash, Role, IsEnabled FROM Users WHERE Username = $username";
            command.Parameters.AddWithValue("$username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var passwordHash = reader.GetString(0);
                var role = reader.GetString(1);
                var isEnabled = reader.GetInt32(2) == 1;

                if (!isEnabled)
                {
                    throw new InvalidOperationException("L'utente non è abilitato. Contattare un amministratore.");
                }

                if (BCrypt.Net.BCrypt.Verify(password, passwordHash))
                {
                    CurrentUser = new User
                    {
                        Username = username,
                        PasswordHash = passwordHash,
                        Role = role,
                        IsEnabled = isEnabled
                    };
                    return true;
                }
            }
            return false;
        }


        public bool ChangePassword(string currentPassword, string newPassword)
        {
            if (!PasswordValidation.ValidatePassword(newPassword, out string errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            if (BCrypt.Net.BCrypt.Verify(currentPassword, CurrentUser.PasswordHash))
            {
                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Users SET PasswordHash = $newPasswordHash WHERE Username = $username";
                    command.Parameters.AddWithValue("$newPasswordHash", newPasswordHash);
                    command.Parameters.AddWithValue("$username", CurrentUser.Username);
                    command.ExecuteNonQuery();
                }
                CurrentUser.PasswordHash = newPasswordHash;
                //DatabaseHelper.LogAudit(CurrentUser.Username, "Password changed");
                return true;
            }
            return false;
        }

        public static bool AddUser(string username, string password, string role)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Users (Username, PasswordHash, Role, IsEnabled) VALUES ($username, $passwordHash, $role, 1)";
            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$passwordHash", BCrypt.Net.BCrypt.HashPassword(password));
            command.Parameters.AddWithValue("$role", role);

            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (SqliteException)
            {
                return false; // Username già esistente
            }
        }

        public static bool RemoveUser(string username)
        {
            if (username == "admin") return false; // Evita la rimozione dell'utente admin predefinito

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Username = $username";
            command.Parameters.AddWithValue("$username", username);
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public static bool UpdateRole(string username, string role)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET Role = $role WHERE Username = $username";
            command.Parameters.AddWithValue("$role", role);
            command.Parameters.AddWithValue("$username", username);
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public static bool VerifyPassword(string username, string password)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT PasswordHash FROM Users WHERE Username = $username";
            command.Parameters.AddWithValue("$username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var passwordHash = reader.GetString(0);
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            return false;
        }

        public static bool UpdatePassword(string username, string newPasswordHash)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET PasswordHash = $newPasswordHash WHERE Username = $username";
            command.Parameters.AddWithValue("$newPasswordHash", newPasswordHash);
            command.Parameters.AddWithValue("$username", username);
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public static bool ToggleEnableUser(string username, bool isEnabled)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET IsEnabled = $isEnabled WHERE Username = $username";
            command.Parameters.AddWithValue("$isEnabled", isEnabled ? 1 : 0);
            command.Parameters.AddWithValue("$username", username);
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public static List<User> GetUsers()
        {
            var users = new List<User>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Username, Role, IsEnabled FROM Users";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Username = reader.GetString(0),
                        Role = reader.GetString(1),
                        IsEnabled = reader.GetInt32(2) == 1
                    });
                }
            }
            return users;
        }
    }
}
