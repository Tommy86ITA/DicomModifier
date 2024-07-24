using DicomModifier.Models;
using DicomModifier.Services;

namespace DicomModifier.Views
{
    public partial class ChangePasswordForm : Form
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Rimuovi i membri privati non letti", Justification = "<In sospeso>")]
        private readonly AuthenticationService _authService;
        private readonly User _user;
        private readonly bool _requireCurrentPassword;

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
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _requireCurrentPassword = requireCurrentPassword;
            LoadForm();
        }

        private void InitializeEvents()
        {
            buttonChangePassword.Click += ButtonChangePassword_Click;
            buttonCancel.Click += ButtonCancel_Click;
        }

        private void LoadForm()
        {
            if (!_requireCurrentPassword)
            {
                textBoxCurrentPassword.Visible = false;
                labelCurrentPassword.Visible = false;
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

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("La nuova password non può essere vuota.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Le nuove password non coincidono.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            DialogResult = DialogResult.OK;
            Close();
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