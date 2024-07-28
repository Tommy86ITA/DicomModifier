// Interfaces/AuthenticationServices.cs

using DicomModifier.Models;
using Microsoft.Data.Sqlite;

namespace DicomModifier.Services
{
    public class AuthenticationService
    {
        // Property to hold the current user
        public User CurrentUser { get; private set; }

        // Constructor to initialize the database and the current user
        public AuthenticationService()
        {
            DatabaseHelper.InitializeDatabase();
            CurrentUser = new User(); // Default initialization
        }

        // Method to authenticate a user
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

                // Check if the user is enabled
                if (!isEnabled)
                {
                    throw new InvalidOperationException("L'utente non è abilitato. Contattare un amministratore.");
                }

                // Verify the password
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

        // Method to add a new user
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
                return false; // Username already exists
            }
        }

        // Method to remove a user
        public static bool RemoveUser(string username)
        {
            if (username == "admin") return false; // Prevent removal of the default admin user

            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Users WHERE Username = $username";
            command.Parameters.AddWithValue("$username", username);
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        // Method to update a user's role
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

        // Method to verify a user's password
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

        // Method to update a user's password
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

        // Method to enable or disable a user
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

        // Method to get a list of all users
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
