// Interfaces/UserValidation.cs

using DicomModifier.Models;

namespace DicomModifier.Services
{
    public static class UserValidation
    {
        /// <summary>
        /// Checks if the user's role can be updated to a new role, ensuring there is at least one enabled administrator.
        /// </summary>
        /// <param name="user">The user whose role is being updated.</param>
        /// <param name="users">List of all users.</param>
        /// <param name="newRole">The new role to be assigned.</param>
        /// <returns>True if the role can be updated, otherwise false.</returns>
        public static bool CanUpdateUserRole(User user, List<User> users, string newRole)
        {
            if (newRole != "Administrator" && users.Count(u => u.Role == "Administrator" && u.IsEnabled) == 1 && user.Role == "Administrator")
            {
                MessageBox.Show("Deve esserci almeno un amministratore abilitato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the user can be deleted, ensuring there is at least one enabled administrator.
        /// </summary>
        /// <param name="user">The user to be deleted.</param>
        /// <param name="users">List of all users.</param>
        /// <returns>True if the user can be deleted, otherwise false.</returns>
        public static bool CanDeleteUser(User user, List<User> users)
        {
            if (user.Role == "Administrator" && users.Count(u => u.Role == "Administrator" && u.IsEnabled) == 1)
            {
                MessageBox.Show("Non puoi eliminare l'unico amministratore abilitato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the provided username is unique among all users.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <param name="users">List of all users.</param>
        /// <returns>True if the username is unique, otherwise false.</returns>
        public static bool IsUsernameUnique(string username, List<User> users)
        {
            if (users.Any(u => u.Username == username))
            {
                MessageBox.Show("Il nome utente è già in uso.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the current user's role can be updated, ensuring the user cannot degrade their role if they are the only enabled administrator.
        /// </summary>
        /// <param name="currentUser">The current user whose role is being updated.</param>
        /// <param name="newRole">The new role to be assigned.</param>
        /// <param name="users">List of all users.</param>
        /// <returns>True if the role can be updated, otherwise false.</returns>
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

        /// <summary>
        /// Checks if the user can be disabled, ensuring there is at least one enabled administrator.
        /// </summary>
        /// <param name="user">The user to be disabled.</param>
        /// <param name="users">List of all users.</param>
        /// <param name="isEnabled">The user's enabled status.</param>
        /// <returns>True if the user can be disabled, otherwise false.</returns>
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
