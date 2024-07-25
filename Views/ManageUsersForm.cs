// Interfaces/ManageUsersForm.cs

using DicomModifier.Services;
using DicomModifier.Models;

namespace DicomModifier.Views
{
    public partial class ManageUsersForm : Form
    {
        private readonly AuthenticationService _authService;
        private List<User> _users;

        public ManageUsersForm(AuthenticationService authService)
        {
            InitializeComponent();
            InitializeEvents();
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _users = [];
            LoadUsers();
        }

        private void InitializeEvents()
        {
            buttonAddUser.Click += ButtonAddUser_Click;
            buttonEditUser.Click += ButtonEditUser_Click;
            buttonDeleteUser.Click += ButtonDeleteUser_Click;
            buttonChangePassword.Click += ButtonChangePassword_Click;
            buttonClose.Click += ButtonClose_Click;
        }

        private void LoadUsers()
        {
            _users = AuthenticationService.GetUsers();
            dataGridViewUsers.Rows.Clear();
            foreach (var user in _users)
            {
                dataGridViewUsers.Rows.Add(user.Username, user.Role, user.IsEnabled);
            }
        }

        private void ButtonAddUser_Click(object? sender, EventArgs e)
        {
            var newUser = new User();
            using var createEditUserForm = new CreateEditUserForm(newUser, false, _authService);
            if (createEditUserForm.ShowDialog() == DialogResult.OK)
            {
                var createdUser = createEditUserForm.GetUser();
                if (AuthenticationService.AddUser(createdUser.Username, createdUser.PasswordHash, createdUser.Role))
                {
                    _users.Add(createdUser);
                    LoadUsers();
                    MessageBox.Show("Utente creato correttamente.", "Creazione utente", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Apri la schermata di modifica della password
                    using var changePasswordForm = new ChangePasswordForm(_authService, createdUser, false);
                    if (changePasswordForm.ShowDialog() != DialogResult.OK)
                    {
                        // Se l'utente esce senza salvare la password, annulla la creazione dell'utente
                        AuthenticationService.RemoveUser(createdUser.Username);
                        _users.RemoveAll(u => u.Username == createdUser.Username);
                        LoadUsers();
                        MessageBox.Show("La creazione dell'utente è stata annullata perché non è stata impostata la password.", "Creazione utente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Impossibile creare l'utente.", "Creazione utente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void ButtonEditUser_Click(object? sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleziona una riga della tabella per modificare un utente.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedRow = dataGridViewUsers.SelectedRows[0];
            var username = selectedRow.Cells["columnUserName"].Value?.ToString();
            if (username == null)
            {
                MessageBox.Show("Nome utente non valido.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var user = _users.Find(u => u.Username == username);

            if (user != null)
            {
                using var createEditUserForm = new CreateEditUserForm(user, true, _authService);
                if (createEditUserForm.ShowDialog() == DialogResult.OK)
                {
                    var updatedUser = createEditUserForm.GetUser();
                    var users = AuthenticationService.GetUsers();
                    if (!UserValidation.CanUpdateUserRole(updatedUser, users, updatedUser.Role) ||
                        !UserValidation.CanUpdateCurrentUserRole(_authService.CurrentUser, updatedUser.Role, users))
                    {
                        LoadUsers(); // Ripristina lo stato originale
                        return;
                    }

                    if (AuthenticationService.UpdateRole(updatedUser.Username, updatedUser.Role) && AuthenticationService.ToggleEnableUser(updatedUser.Username, updatedUser.IsEnabled))
                    {
                        LoadUsers();
                        MessageBox.Show("Utente aggiornato con successo.", "Modifica utente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Impossibile aggiornare l'utente.", "Modifica utente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }


        private void ButtonDeleteUser_Click(object? sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleziona una riga della tabella per eliminare un utente.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var username = dataGridViewUsers.SelectedRows[0].Cells["columnUserName"].Value?.ToString();
            if (username == null)
            {
                MessageBox.Show("Seleziona un utente.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var user = _users.Find(u => u.Username == username);

            if (user == null || !UserValidation.CanDeleteUser(user, _users))
            {
                return;
            }

            var confirmResult = MessageBox.Show($"Sei sicuro di voler eliminare l'utente {username}?", "Conferma Eliminazione", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.Yes)
            {
                if (AuthenticationService.RemoveUser(username))
                {
                    _users.RemoveAll(u => u.Username == username);
                    LoadUsers();
                    MessageBox.Show("Utente eliminato con successo.", "Eliminazione utente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Errore nell'eliminazione dell'utente.", "Eliminazione utente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ButtonChangePassword_Click(object? sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleziona una riga della tabella per cambiare la password di un utente.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedRow = dataGridViewUsers.SelectedRows[0];
            var username = selectedRow.Cells["columnUserName"].Value?.ToString();
            if (username == null)
            {
                MessageBox.Show("Nome utente non valido.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var user = _users.Find(u => u.Username == username);
            if (user == null)
            {
                MessageBox.Show("Utente non trovato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (username == _authService.CurrentUser.Username)
            {
                using var changePasswordForm = new ChangePasswordForm(_authService, user, true); // Passa true per richiedere la password attuale
                if (changePasswordForm.ShowDialog() == DialogResult.OK)
                {
                    var updatedUser = changePasswordForm.GetUser();
                    AuthenticationService.UpdatePassword(updatedUser.Username, updatedUser.PasswordHash);
                    MessageBox.Show("Password aggiornata con successo.", "Modifica password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                using var changePasswordForm = new ChangePasswordForm(_authService, user);
                if (changePasswordForm.ShowDialog() == DialogResult.OK)
                {
                    var updatedUser = changePasswordForm.GetUser();
                    AuthenticationService.UpdatePassword(updatedUser.Username, updatedUser.PasswordHash);
                    MessageBox.Show("Password aggiornata con successo.", "Modifica password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ButtonClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}
