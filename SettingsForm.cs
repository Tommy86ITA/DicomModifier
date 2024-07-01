using System;
using System.Windows.Forms;

namespace DicomModifier
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
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
            // Effettua i controlli sui dati inseriti
            if (!int.TryParse(textBoxServerPort.Text, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("Inserisci una porta valida (1-65535).", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBoxTimeout.Text, out int timeout) || timeout < 0)
            {
                MessageBox.Show("Inserisci un timeout valido (>= 0).", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
