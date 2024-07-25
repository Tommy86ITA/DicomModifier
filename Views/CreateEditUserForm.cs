using DicomModifier.Models;
using DicomModifier.Services;

namespace DicomModifier.Views
{
    public partial class CreateEditUserForm : Form
    {
        private readonly User _user;
        private readonly bool _isEdit;
        private readonly AuthenticationService _authService;

        public CreateEditUserForm(User user, bool isEdit = false, AuthenticationService? authService = null)
        {
            InitializeComponent();
            InitializeEvents();
            _user = user;
            _isEdit = isEdit;
            _authService = authService!;
            LoadUserData();
        }

        private void InitializeEvents()
        {
            buttonCancel.Click += ButtonCancel_Click;
            buttonSave.Click += ButtonSave_Click;
        }

        private void LoadUserData()
        {
            if (_isEdit)
            {
                this.Text = "Modifica utente";
                textBoxUsername.Text = _user.Username;
                textBoxUsername.Enabled = false; // Non consentire la modifica del nome utente
                comboBoxRole.SelectedItem = _user.Role;
                checkBoxEnableUser.Checked = _user.IsEnabled; // Carica lo stato di abilitazione
                if (_user.Username == _authService.CurrentUser.Username)
                {
                    checkBoxEnableUser.Enabled = false; // Disabilita il checkbox se l'utente corrente coincide con l'utente loggato
                }
            }
            else
            {
                this.Text = "Creazione utente";
            }
        }

        private void ButtonSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxUsername.Text))
            {
                MessageBox.Show("Il nome utente non può essere vuoto.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBoxRole.SelectedItem == null)
            {
                MessageBox.Show("Devi selezionare un ruolo.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedRole = comboBoxRole.SelectedItem.ToString()!;
            if (_isEdit && _user.Username == _authService.CurrentUser.Username && selectedRole != "Administrator")
            {
                MessageBox.Show("Non puoi degradare il tuo ruolo.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_isEdit && !UserValidation.IsUsernameUnique(textBoxUsername.Text, AuthenticationService.GetUsers()))
            {
                MessageBox.Show("Il nome utente esiste già.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var users = AuthenticationService.GetUsers();
            if (!UserValidation.CanUpdateUserRole(_user, users, selectedRole))
            {
                return;
            }

            _user.Username = textBoxUsername.Text;
            _user.Role = selectedRole;
            _user.IsEnabled = checkBoxEnableUser.Checked; // Imposta lo stato di abilitazione

            if (_isEdit)
            {
                if (!UserValidation.CanUpdateCurrentUserRole(_authService.CurrentUser, selectedRole, users))
                {
                    return;
                }

                if (!UserValidation.CanDisableUser(_user, users, _user.IsEnabled))
                {
                    MessageBox.Show("Ci deve essere almeno un utente Administrator abilitato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

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
