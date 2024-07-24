// Models/User.cs

namespace DicomModifier.Models
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsEnabled { get; set; }

        // Costruttore principale
        public User(string username, string passwordHash, string role, bool isEnabled)
        {
            Username = username;
            PasswordHash = passwordHash;
            Role = role;
            IsEnabled = isEnabled;
        }

        // Costruttore predefinito per inizializzazione
        public User()
        {
            Username = string.Empty;
            PasswordHash = string.Empty;
            Role = string.Empty;
            IsEnabled = true;
        }
    }
}
