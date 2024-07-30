// Interfaces/ChangePasswordForm.cs

using DicomModifier.Models;
using DicomModifier.Services;

namespace DicomModifier.Views
{
    public partial class ChangePasswordForm : Form
    {
        private readonly User _user;
        private readonly bool _requireCurrentPassword;
        private readonly DatabaseHelper _databaseHelper;
        private readonly AuthenticationService _authService;

        // Costruttore per l'utente corrente
        public ChangePasswordForm(AuthenticationService authService)
            : this(authService, authService.CurrentUser, true)
        {
        }

        // Costruttore principale
        public ChangePasswordForm(AuthenticationService authService, User user, bool requireCurrentPassword = false)
        {
            InitializeComponent();
            InitializeEvents();
            _databaseHelper = new DatabaseHelper();
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _requireCurrentPassword = requireCurrentPassword;
            _authService = authService;
            LoadForm();
        }

        private void InitializeEvents()
        {
            buttonChangePassword.Click += ButtonChangePassword_Click;
            buttonCancel.Click += ButtonCancel_Click;
            textBoxNewPassword.TextChanged += TextBoxNewPassword_TextChanged;
        }

        private void TextBoxNewPassword_TextChanged(object? sender, EventArgs e)
        {
            UpdatePasswordValidationIndicators();
        }

        private void LoadForm()
        {
            UpdatePasswordValidationIndicators();
            if (!_requireCurrentPassword)
            {
                textBoxCurrentPassword.Visible = false;
                labelCurrentPassword.Visible = false;
                labelUser.Visible = true;
                labelUser.Text = $"Stai modificando la password dell'utente: \n{_user.Username}";
            }
        }

        private void ButtonChangePassword_Click(object? sender, EventArgs e)
        {
            if (_requireCurrentPassword)
            {
                string currentPassword = textBoxCurrentPassword.Text;
                if (string.IsNullOrWhiteSpace(currentPassword) || !AuthenticationService.VerifyPassword(_user.Username, currentPassword))
                {
                    MessageBox.Show("La password attuale non è corretta.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string newPassword = textBoxNewPassword.Text;
            string confirmPassword = textBoxConfirmNewPassword.Text;

            if (!PasswordValidation.ValidatePassword(newPassword, out string errorMessage))
            {
                MessageBox.Show(errorMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Le nuove password non coincidono.", "Modifica password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            AuthenticationService.UpdatePassword(_user.Username, _user.PasswordHash);
            MessageBox.Show("Password aggiornata con successo.", "Modifica password", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _databaseHelper.LogAudit(_authService.CurrentUser.Username, EventMapping.EventType.PasswordChanged, $"Modifica la password per l'utente {_user.Username}");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void UpdatePasswordValidationIndicators()
        {
            string password = textBoxNewPassword.Text;
            labelLength.ForeColor = password.Length >= 8 ? Color.Green : Color.Red;
            labelUpperCase.ForeColor = password.Any(char.IsUpper) ? Color.Green : Color.Red;
            labelLowerCase.ForeColor = password.Any(char.IsLower) ? Color.Green : Color.Red;
            labelDigit.ForeColor = password.Any(char.IsDigit) ? Color.Green : Color.Red;
            labelSpecialChar.ForeColor = password.Any(ch => !char.IsLetterOrDigit(ch)) ? Color.Green : Color.Red;
        }

        private void ButtonCancel_Click(object? sender, EventArgs e)
        {
            Close();
        }

        public User GetUser()
        {
            return _user ?? throw new InvalidOperationException("L'utente non è stato inizializzato correttamente.");
        }
    }
}