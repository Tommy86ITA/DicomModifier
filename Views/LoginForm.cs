﻿// Interfaces/LoginForm.cs

using DicomModifier.Models;
using DicomModifier.Services;
using System.Reflection;

namespace DicomModifier.Views
{
    public partial class LoginForm : Form
    {
        private readonly AuthenticationService authService;
        private readonly DatabaseHelper _databaseHelper;

        /// <summary>
        /// Costruttore per il form di login. Inizializza i componenti e imposta la versione.
        /// </summary>
        /// <param name="authService">Servizio di autenticazione iniettato.</param>
        public LoginForm(AuthenticationService authService)
        {
            InitializeComponent();
            this.authService = authService;
            _databaseHelper = new DatabaseHelper();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = version != null ? version.ToString() : "Versione non disponibile";
            labelVersion.Text = $"v. {versionString} - Copyright (c) 2024 Thomas Amaranto";

            // Inizializza gli eventi dei pulsanti
            buttonLogin.Click += ButtonLogin_Click;
            buttonQuit.Click += ButtonQuit_Click;
        }

        /// <summary>
        /// Gestore dell'evento per il pulsante di login.
        /// Verifica le credenziali dell'utente e chiude il form se l'autenticazione ha successo.
        /// </summary>
        private void ButtonLogin_Click(object? sender, EventArgs e)
        {
            string username = textBoxUsername.Text;
            string password = textBoxPassword.Text;
            try
            {
                if (authService.Authenticate(username, password))
                {
                    MessageBox.Show("Accesso consentito, credenziali verificate!", "Accesso consentito!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _databaseHelper.LogAudit(username, EventMapping.EventType.LoginSuccessful);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Accesso negato. Verifica che le credenziali siano corrette.", "Accesso negato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _databaseHelper.LogAudit(username, EventMapping.EventType.LoginFailed_InvalidCredentials);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Accesso negato {ex.Message}", "Accesso negato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _databaseHelper.LogAudit(username, EventMapping.EventType.LoginFailed_UserDisabled, null);
            }
        }

        /// <summary>
        /// Gestore dell'evento per il pulsante di uscita.
        /// Chiude l'applicazione.
        /// </summary>
        private void ButtonQuit_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}