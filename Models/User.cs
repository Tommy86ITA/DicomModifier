// Models/User.cs

namespace DicomModifier.Models
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Usa una funzione di hashing sicura per le password
        public string Role { get; set; } // Es. "Admin", "Technician", "Operator"
    }
}
