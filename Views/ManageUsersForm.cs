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
            buttonAddUser.Click += ButtonAddUser_Click;
            buttonEditUser.Click += ButtonEditUser_Click;
            buttonDeleteUser.Click += ButtonDeleteUser_Click;
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
            using (var createEditUserForm = new CreateEditUserForm(newUser))
            {
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
        }

        private void ButtonEditUser_Click(object? sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridViewUsers.SelectedRows[0];
                var username = selectedRow.Cells["columnUserName"].Value.ToString();
                var user = _users.Find(u => u.Username == username);

                if (user != null)
                {
                    using (var createEditUserForm = new CreateEditUserForm(user, true))
                    {
                        if (createEditUserForm.ShowDialog() == DialogResult.OK)
                        {
                            var updatedUser = createEditUserForm.GetUser();
                            if (AuthenticationService.UpdateRole(updatedUser.Username, updatedUser.Role) && AuthenticationService.ToggleEnableUser(updatedUser.Username, updatedUser.IsEnabled))
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
            }
        }

        private void ButtonDeleteUser_Click(object? sender, EventArgs e)
        {
            if (dataGridViewUsers.SelectedRows.Count > 0)
            {
                var username = dataGridViewUsers.SelectedRows[0].Cells["columnUserName"].Value.ToString();
                if (AuthenticationService.RemoveUser(username))
                {
                    _users.RemoveAll(u => u.Username == username);
                    LoadUsers();
                    MessageBox.Show("User deleted successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to delete user.");
                }
            }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
