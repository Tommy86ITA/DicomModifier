// Interfaces/CreateEditUserForm.cs

using System;
using System.Windows.Forms;
using DicomModifier.Models;

namespace DicomModifier.Views
{
    public partial class CreateEditUserForm : Form
    {
        private User _user;
        private bool _isEdit;

        public CreateEditUserForm(User user, bool isEdit = false)
        {
            InitializeComponent();
            InitializeEvents();
            _user = user;
            _isEdit = isEdit;
            LoadUserData();
        }

        private void InitializeEvents()
        {
            buttonCancel.Click += ButtonCancel_Click;
            buttonSave.Click += ButtonChangePassword_Click;
        }

        private void LoadUserData()
        {
            if (_isEdit)
            {
                textBoxUsername.Text = _user.Username;
                textBoxUsername.Enabled = false; // Non consentire la modifica del nome utente
                comboBoxRole.SelectedItem = _user.Role;
            }
        }

        private void ButtonChangePassword_Click(object sender, EventArgs e)
        {
            string newPassword = textBoxPassword.Text;
            string confirmPassword = textBoxConfirmPassword.Text;

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("The password cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("The passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_isEdit && string.IsNullOrWhiteSpace(textBoxUsername.Text))
            {
                MessageBox.Show("Username cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _user.Username = textBoxUsername.Text;
            _user.Role = comboBoxRole.SelectedItem.ToString();
            _user.IsEnabled = true; // Sempre abilitato di default
            _user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword); // Hash della nuova password

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public User GetUser()
        {
            return _user;
        }
    }
}

