﻿// Interfaces/CreateEditUserForm.cs

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
            }
            else
            {
                this.Text = "Creazione utente";
            }
        }

        private void ButtonSave_Click(object? sender, EventArgs e)
        {
            if (!_isEdit && string.IsNullOrWhiteSpace(textBoxUsername.Text))
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
                return;
            }

            _user.Username = textBoxUsername.Text;
            _user.Role = selectedRole;
            _user.IsEnabled = true; // Sempre abilitato di default

            if (!UserValidation.CanUpdateUserRole(_user, AuthenticationService.GetUsers(), selectedRole))
            {
                return;
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