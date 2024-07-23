using System.Collections.Generic;
using System.Linq;
using BCrypt.Net; // Assicurati che questa direttiva using sia presente
using DicomModifier.Models;

namespace DicomModifier.Services
{
    public class AuthenticationService
    {
        private List<User> users = new List<User>();

        public AuthenticationService()
        {
            // Aggiungi utenti di esempio. In un'applicazione reale, carica utenti da un database o da un file.
            users.Add(new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "Admin" });
            users.Add(new User { Username = "tech", PasswordHash = BCrypt.Net.BCrypt.HashPassword("tech123"), Role = "Technician" });
        }

        public bool Authenticate(string username, string password)
        {
            var user = users.FirstOrDefault(u => u.Username == username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        public bool ChangePassword(string currentPassword, string newPassword)
        {
            if (BCrypt.Net.BCrypt.Verify(currentPassword, CurrentUser.PasswordHash))
            {
                CurrentUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                return true;
            }
            return false;
        }
        public User CurrentUser { get; private set; }
    }
}
