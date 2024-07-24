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
            _authService = authService;
            LoadUsers();
        }

        private void InitializeEvents()
        {
            dataGridViewUsers.CellValueChanged += DataGridViewUsers_CellValueChanged;
            dataGridViewUsers.UserDeletingRow += DataGridViewUsers_UserDeletingRow;
            dataGridViewUsers.DataError += DataGridViewUsers_DataError;
            buttonAddUser.Click += ButtonAddUser_Click;
            buttonEditUser.Click += ButtonEditUser_Click;
            buttonChangePassword.Click += ButtonChangePassword_Click;
            buttonDeleteUser.Click += ButtonDeleteUser_Click;
            buttonClose.Click += ButtonClose_Click;
        }
        private void LoadUsers()
        {
            _users = _authService.GetUsers();
            dataGridViewUsers.Rows.Clear();
            foreach (var user in _users)
            {
                // Assicurati che il ruolo sia "Administrator" o "Technician"
                if (user.Role != "Administrator" && user.Role != "Technician")
                {
                    user.Role = "Technician"; // Imposta un valore predefinito se il ruolo è invalido
                }

                dataGridViewUsers.Rows.Add(user.Username, user.Role, user.IsEnabled);
            }
        }

        private void DataGridViewUsers_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridViewUsers.Rows[e.RowIndex];
                var username = row.Cells["columnUserName"].Value.ToString();
                var role = row.Cells["roleColumn"].Value.ToString();
                var isEnabled = (bool)row.Cells["enabledColumn"].Value;

                var user = _users.Find(u => u.Username == username);
                if (user != null)
                {
                    user.Role = role;
                    user.IsEnabled = isEnabled;

                    _authService.UpdateRole(user.Username, user.Role);
                    _authService.ToggleEnableUser(user.Username, user.IsEnabled);
                }
                else
                {
                    // Se l'utente non esiste, significa che stiamo creando un nuovo utente
                    var newUser = new User
                    {
                        Username = username,
                        Role = role,
                        IsEnabled = isEnabled,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") // Usa una password predefinita per ora
                    };
                    if (_authService.AddUser(newUser.Username, newUser.PasswordHash, newUser.Role))
                    {
                        _users.Add(newUser);
                        MessageBox.Show("User added successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to add user.");
                    }
                }

                // Validazione per garantire che ci sia almeno un Administrator abilitato
                ValidateAdminUsers();
            }
        }

        private void DataGridViewUsers_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var username = e.Row.Cells["columnUserName"].Value.ToString();
            var user = _users.Find(u => u.Username == username);
            if (user != null)
            {
                if (user.Role == "Administrator" && _users.Count(u => u.Role == "Administrator" && u.IsEnabled) == 1)
                {
                    MessageBox.Show("Non puoi eliminare l'ultimo utente Administrator abilitato.");
                    e.Cancel = true;
                }
                else
                {
                    _authService.RemoveUser(username);
                    _users.Remove(user);
                }
            }
        }

        private void DataGridViewUsers_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Si è verificato un errore nella DataGridView. Verifica i dati inseriti.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ValidateAdminUsers()
        {
            if (_users.Count(u => u.Role == "Administrator") == 0)
            {
                MessageBox.Show("Deve esserci almeno un utente con il ruolo Administrator.");
            }
            if (_users.Count(u => u.Role == "Administrator" && u.IsEnabled) == 0)
            {
                MessageBox.Show("Deve esserci almeno un utente Administrator abilitato.");
            }
        }

        private void ButtonClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void ButtonAddUser_Click(object? sender, EventArgs e)
        {
            var newUser = new User
            {
                Username = "NewUser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Role = "Technician",
                IsEnabled = true
            };

            if (_authService.AddUser(newUser.Username, "password", newUser.Role))
            {
                LoadUsers();
                MessageBox.Show("User added successfully.");
            }
            else
            {
                MessageBox.Show("Failed to add user.");
            }
        }

        private void ButtonEditUser_Click(object? sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewUsers.SelectedRows[0];
                var username = selectedRow.Cells[0].Value.ToString();

                var user = _users.Find(u => u.Username == username);
                if (user != null)
                {
                    user.Role = selectedRow.Cells[2].Value.ToString();
                    user.IsEnabled = (bool)selectedRow.Cells[3].Value;

                    if (_authService.UpdateRole(user.Username, user.Role) && _authService.ToggleEnableUser(user.Username, user.IsEnabled))
                    {
                        LoadUsers();
                        MessageBox.Show("User updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to update user.");
                    }
                }
            }
        }

        private void ButtonDeleteUser_Click(object? sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                var username = dataGridViewUsers.SelectedRows[0].Cells[0].Value.ToString();
                if (_authService.RemoveUser(username))
                {
                    LoadUsers();
                    MessageBox.Show("User deleted successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to delete user.");
                }
            }
        }

        private void ButtonChangePassword_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewUsers.SelectedRows[0];
                var username = selectedRow.Cells["columnUserName"].Value.ToString();
                var user = _users.Find(u => u.Username == username);

                using (var createEditPasswordForm = new CreateEditPasswordForm(user))
                {
                    if (createEditPasswordForm.ShowDialog() == DialogResult.OK)
                    {
                        var updatedUser = createEditPasswordForm.GetUser();
                        _authService.UpdatePassword(updatedUser.Username, updatedUser.PasswordHash);
                        MessageBox.Show("Password updated successfully.");
                    }
                }
            }
        }
    }
}

