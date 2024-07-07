﻿using DicomModifier.Controllers;
using DicomModifier.Models;

namespace DicomModifier
{
    public partial class SettingsForm : Form
    {
        private readonly PACSSettings _settings;
        private readonly SettingsController _settingsController;
        private readonly ProgressManager _progressManager;

        public SettingsForm(PACSSettings settings, SettingsController settingsController)
        {
            InitializeComponent();
            InitializeEvents();
            _settings = settings;
            _settingsController = settingsController;
            _progressManager = new ProgressManager(_settingsController.GetMainForm());
            LoadSettings(_settings);
        }

        private void InitializeEvents()
        {
            // Inizializza gli eventi
            textBoxServerPort.KeyPress += TextBoxServerPort_KeyPress;
            textBoxTimeout.KeyPress += TextBoxTimeout_KeyPress;
            textBoxServerIP.KeyPress += TextBoxServerIP_KeyPress;
            textBoxServerPort.TextChanged += TextBoxServerPort_TextChanged;
            textBoxTimeout.TextChanged += TextBoxTimeout_TextChanged;
            textBoxServerIP.TextChanged += TextBoxServerIP_TextChanged;
            buttonSave.Click += buttonSave_Click;
            buttonCancel.Click += buttonCancel_Click;
            buttonEchoTest.Click += buttonCEcho_Click;
        }

        public void LoadSettings(PACSSettings settings)
        {
            textBoxServerIP.Text = settings.ServerIP;
            textBoxServerPort.Text = settings.ServerPort;
            textBoxAETitle.Text = settings.AETitle;
            textBoxTimeout.Text = settings.Timeout;
            textBoxLocalAETitle.Text = settings.LocalAETitle;
        }

        public PACSSettings GetSettings()
        {
            return new PACSSettings
            {
                ServerIP = textBoxServerIP.Text,
                ServerPort = textBoxServerPort.Text,
                AETitle = textBoxAETitle.Text,
                Timeout = textBoxTimeout.Text,
                LocalAETitle = textBoxLocalAETitle.Text
            };
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            _settings.ServerIP = textBoxServerIP.Text;
            _settings.ServerPort = textBoxServerPort.Text;
            _settings.AETitle = textBoxAETitle.Text;
            _settings.Timeout = textBoxTimeout.Text;
            _settings.LocalAETitle = textBoxLocalAETitle.Text;
            _settingsController.SaveSettings(_settings);

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void buttonCEcho_Click(object sender, EventArgs e)
        {
            buttonEchoTest.Text = "Test in corso...";
            this.Enabled = false;
            var communicator = new PACSCommunicator(_settings, _progressManager);
            bool success = await communicator.SendCEcho();
            if (success)
            {
                MessageBox.Show("C-ECHO riuscito!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("C-ECHO fallito.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            buttonEchoTest.Text = "Esegui C-ECHO";
            this.Enabled = true;
        }

        private void TextBoxServerPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Consente solo l'inserimento di cifre
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBoxTimeout_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Consente solo l'inserimento di cifre
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBoxServerIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Consente solo l'inserimento di cifre e punti
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void TextBoxServerPort_TextChanged(object sender, EventArgs e)
        {
            // Controlla se il numero è nel range corretto
            if (int.TryParse(textBoxServerPort.Text, out int port))
            {
                if (port < 1 || port > 65535)
                {
                    textBoxServerPort.BackColor = Color.Red;
                }
                else
                {
                    textBoxServerPort.BackColor = Color.Green;
                }
            }
            else
            {
                textBoxServerPort.BackColor = Color.Red;
            }
        }

        private void TextBoxTimeout_TextChanged(object sender, EventArgs e)
        {
            // Controlla se il timeout è valido
            if (int.TryParse(textBoxTimeout.Text, out int timeout))
            {
                if (timeout < 0)
                {
                    textBoxTimeout.BackColor = Color.Red;
                }
                else
                {
                    textBoxTimeout.BackColor = Color.Green;
                }
            }
            else
            {
                textBoxTimeout.BackColor = Color.Red;
            }
        }

        private void TextBoxServerIP_TextChanged(object sender, EventArgs e)
        {
            // Controlla se l'indirizzo IP è valido
            var ipSegments = textBoxServerIP.Text.Split('.');
            if (ipSegments.Length == 4 && ipSegments.All(segment => int.TryParse(segment, out int num) && num >= 0 && num <= 255))
            {
                textBoxServerIP.BackColor = Color.Green;
            }
            else
            {
                textBoxServerIP.BackColor = Color.Red;
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty(textBoxAETitle.Text) ||
                string.IsNullOrEmpty(textBoxServerIP.Text) ||
                string.IsNullOrEmpty(textBoxServerPort.Text) ||
                string.IsNullOrEmpty(textBoxTimeout.Text) ||
                string.IsNullOrEmpty(textBoxLocalAETitle.Text))
            {
                MessageBox.Show("Per favore, compila tutti i campi.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(textBoxServerPort.Text, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("Inserisci una porta valida (1-65535).", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!int.TryParse(textBoxTimeout.Text, out int timeout) || timeout < 0)
            {
                MessageBox.Show("Inserisci un timeout valido (>= 0).", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var ipSegments = textBoxServerIP.Text.Split('.');
            if (ipSegments.Length != 4 || ipSegments.Any(segment => !int.TryParse(segment, out int num) || num < 0 || num > 255))
            {
                MessageBox.Show("Inserisci un indirizzo IP valido.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
