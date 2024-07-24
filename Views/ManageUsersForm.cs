// Interfaces/ManageUsersForm.cs

using System;
using System.Collections.Generic;
using System.Windows.Forms;
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
            using var createEditUserForm = new CreateEditUserForm(newUser);
            if (createEditUserForm.ShowDialog() == DialogResult.OK)
            {
                var createdUser = createEditUserForm.GetUser();
                if (AuthenticationService.AddUser(createdUser.Username, createdUser.PasswordHash, createdUser.Role))
                {
                    _users.Add(createdUser);
                    LoadUsers();
                    MessageBox.Show("User added successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to add user.");
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
                        MessageBox.Show("Utente aggiornato con successo.");
                    }
                    else
                    {
                        MessageBox.Show("Errore nell'aggiornamento dell'utente.");
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
                    MessageBox.Show("Utente eliminato con successo.");
                }
                else
                {
                    MessageBox.Show("Errore nell'eliminazione dell'utente.");
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
                    MessageBox.Show("Password aggiornata con successo.");
                }
            }
            else
            {
                using var changePasswordForm = new ChangePasswordForm(_authService, user);
                if (changePasswordForm.ShowDialog() == DialogResult.OK)
                {
                    var updatedUser = changePasswordForm.GetUser();
                    AuthenticationService.UpdatePassword(updatedUser.Username, updatedUser.PasswordHash);
                    MessageBox.Show("Password aggiornata con successo.");
                }
            }
        }



        private void ButtonClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}
