using System;
using System.Windows.Forms;
using DicomModifier.Models;

namespace DicomModifier.Views
{
    public partial class CreateEditPasswordForm : Form
    {
        private User _user;

        public CreateEditPasswordForm(User user)
        {
            InitializeComponent();
            InitializeEvents();
            _user = user;
        }

        private void InitializeEvents()
        {
            buttonCancel.Click += ButtonCancel_Click;
            buttonSavePassword.Click += ButtonSavePassword_Click;
        }

        private void ButtonSavePassword_Click(object? sender, EventArgs e)
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

            _user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword); // Hash della nuova password
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, EventArgs e)
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
