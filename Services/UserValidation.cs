// Interfaces/UserValidation.cs

using DicomModifier.Models;

namespace DicomModifier.Services
{
    public static class UserValidation
    {
        public static bool CanUpdateUserRole(User user, List<User> users, string newRole)
        {
            if (newRole != "Administrator" && users.Count(u => u.Role == "Administrator" && u.IsEnabled) == 1 && user.Role == "Administrator")
            {
                MessageBox.Show("Deve esserci almeno un amministratore abilitato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public static bool CanDeleteUser(User user, List<User> users)
        {
            if (user.Role == "Administrator" && users.Count(u => u.Role == "Administrator" && u.IsEnabled) == 1)
            {
                MessageBox.Show("Non puoi eliminare l'unico amministratore abilitato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public static bool IsUsernameUnique(string username, List<User> users)
        {
            if (users.Any(u => u.Username == username))
            {
                MessageBox.Show("Il nome utente è già in uso.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public static bool CanUpdateCurrentUserRole(User currentUser, string newRole, List<User> users)
        {
            if (currentUser.Role == "Administrator" && newRole != "Administrator" &&
                users.Count(u => u.Role == "Administrator" && u.IsEnabled) == 1)
            {
                MessageBox.Show("Non puoi degradare il tuo ruolo se sei l'unico amministratore abilitato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public static bool CanDisableUser(User user, List<User> users, bool isEnabled)
        {
            if (user.Role == "Administrator" && !isEnabled)
            {
                var adminCount = users.Count(u => u.Role == "Administrator" && u.IsEnabled);
                if (adminCount <= 1)
                {
                    MessageBox.Show("Ci deve essere almeno un utente Administrator abilitato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
    }
}