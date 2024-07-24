// Interfaces/ChangePasswordForm.cs

using DicomModifier.Services;

namespace DicomModifier.Views
{
    public partial class ChangePasswordForm : Form
    {
        private readonly AuthenticationService _authService;

        public ChangePasswordForm(AuthenticationService authService)
        {
            InitializeComponent();
            InitializeEvents();
            _authService = authService;
        }

        private void InitializeEvents()
        {
            buttonChangePassword.Click += ButtonChangePassword_Click;
            buttonCancel.Click += ButtonCancel_Click;
        }

        private void ButtonChangePassword_Click(object? sender, EventArgs e)
        {
            string currentPassword = textBoxCurrentPassword.Text;
            string newPassword = textBoxNewPassword.Text;
            string confirmNewPassword = textBoxConfirmNewPassword.Text;

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("The new password cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (newPassword != confirmNewPassword)
            {
                MessageBox.Show("The new passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool result = _authService.ChangePassword(currentPassword, newPassword);
            if (result)
            {
                MessageBox.Show("Password changed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("The current password is incorrect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonCancel_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}