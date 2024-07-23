// Interfaces/SettingsForm.cs

using DicomModifier.Controllers;
using DicomModifier.Models;
using System.ComponentModel;

namespace DicomModifier
{
    public partial class SettingsForm : Form
    {
        private readonly PACSSettings _settings;
        private readonly SettingsController _settingsController;
        private readonly UIController _uiController;
        private readonly ToolTip toolTip;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsForm"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="settingsController">The settings controller.</param>
        /// <param name="uiController">The UI controller.</param>
        public SettingsForm(PACSSettings settings, SettingsController settingsController, UIController uiController)
        {
            InitializeComponent();
            InitializeEvents();
            this.HelpButtonClicked += this.SettingsForm_HelpButtonClicked;
            _settings = settings;
            _settingsController = settingsController;
            _uiController = uiController;
            LoadSettings(_settings);
            ApplyStyles();
            toolTip = new ToolTip();
            InitializeTooltips();
            ValidateFields(); // iniziale validazione dei campi
        }

        /// <summary>
        /// Initializes the events.
        /// </summary>
        private void InitializeEvents()
        {
            // Inizializza gli eventi
            textBoxServerPort.KeyPress += TextBoxServerPort_KeyPress;
            textBoxTimeout.KeyPress += TextBoxTimeout_KeyPress;
            textBoxServerIP.KeyPress += TextBoxServerIP_KeyPress;
            textBoxServerPort.TextChanged += TextBoxServerPort_TextChanged;
            textBoxTimeout.TextChanged += TextBoxTimeout_TextChanged;
            textBoxServerIP.TextChanged += TextBoxServerIP_TextChanged;
            textBoxAETitle.TextChanged += TextBox_TextChanged;
            textBoxLocalAETitle.TextChanged += TextBox_TextChanged;
            buttonSave.Click += ButtonSave_Click;
            buttonCancel.Click += ButtonCancel_Click;
            buttonEchoTest.Click += ButtonCEcho_Click;
            buttonSave.EnabledChanged += Button_EnabledChanged;
            buttonEchoTest.EnabledChanged += Button_EnabledChanged;
        }

        private void InitializeTooltips()
        {
            toolTip.SetToolTip(buttonCancel, "Annulla la modifica");
            toolTip.SetToolTip(buttonSave, "Salva le impostazioni");
            toolTip.SetToolTip(buttonEchoTest, "Esegue il test C-ECHO utilizzando le impostazioni correnti");
        }

        private void SettingsForm_HelpButtonClicked(object? sender, CancelEventArgs e)
        {
            UIController.ShowHelp();
        }

        private void Button_EnabledChanged(object? sender, EventArgs e)
        {
            ApplyStyles();
        }

        private void ApplyStyles()
        {
            // Imposta lo stile dei controlli
            this.BackColor = Color.White;

            foreach (Control control in this.Controls)
            {
                ApplyControlStyle(control);
            }
        }

        private void ApplyControlStyle(Control control)
        {
            if (control is Button button)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.ForeColor = Color.Black;
                button.Font = new Font("Segoe UI", 10, FontStyle.Regular);

                if (button == buttonSave || button == buttonEchoTest)
                {
                    button.BackColor = Color.DodgerBlue;
                    button.ForeColor = Color.White;
                }
                else if (button == buttonCancel)
                {
                    button.BackColor = Color.LightCoral;
                    button.ForeColor = Color.White;
                }

                // Gestione colore pulsanti disabilitati
                if (!button.Enabled)
                {
                    button.BackColor = Color.LightGray;
                    button.ForeColor = Color.Gray;
                }
            }
            else if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }

            // Applicare lo stile ai controlli figli
            foreach (Control childControl in control.Controls)
            {
                ApplyControlStyle(childControl);
            }
        }

        /// <summary>
        /// Loads the settings to the corresponding textbox
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public void LoadSettings(PACSSettings settings)
        {
            textBoxServerIP.Text = settings.ServerIP;
            textBoxServerPort.Text = settings.ServerPort;
            textBoxAETitle.Text = settings.AETitle;
            textBoxTimeout.Text = settings.Timeout;
            textBoxLocalAETitle.Text = settings.LocalAETitle;
        }

        /// <summary>
        /// Gets the settings from the textboxes.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Starts the settings saving procedure when clickink the "Save" button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        /// <returns></returns>
        private void ButtonSave_Click(object? sender, EventArgs e)
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

        /// <summary>
        /// Closes the settings window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private void ButtonCancel_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Buttons the c echo click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        private async void ButtonCEcho_Click(object? sender, EventArgs e)
        {
            buttonEchoTest.Text = "Test in corso...";
            buttonEchoTest.Enabled = false;
            panelEchoStatus.BackColor = Color.Yellow;
            this.Enabled = false;

            PACSSettings testSettings = GetSettings();
            PACSCommunicator communicator = new(testSettings, _uiController);
            bool success = await communicator.SendCEcho();

            if (success)
            {
                panelEchoStatus.BackColor = Color.Green;
                MessageBox.Show("C-ECHO riuscito!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                panelEchoStatus.BackColor = Color.Red;
                MessageBox.Show("C-ECHO fallito.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            buttonEchoTest.Text = "Esegui C-ECHO";
            buttonEchoTest.Enabled = true;
            this.Enabled = true;
        }

        /// <summary>
        /// Texts the box server port key press.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private void TextBoxServerPort_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Consente solo l'inserimento di cifre
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Texts the box timeout key press.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private void TextBoxTimeout_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Consente solo l'inserimento di cifre
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Texts the box server ip key press.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private void TextBoxServerIP_KeyPress(object? sender, KeyPressEventArgs e)
        {
            // Consente solo l'inserimento di cifre e punti
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Texts the box server port text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private void TextBoxServerPort_TextChanged(object? sender, EventArgs e)
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
                    textBoxServerPort.BackColor = Color.LightGreen;
                }
            }
            else
            {
                textBoxServerPort.BackColor = Color.Red;
            }
            ValidateFields();
        }

        /// <summary>
        /// Texts the box timeout text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private void TextBoxTimeout_TextChanged(object? sender, EventArgs e)
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
                    textBoxTimeout.BackColor = Color.LightGreen;
                }
            }
            else
            {
                textBoxTimeout.BackColor = Color.Red;
            }
            ValidateFields();
        }

        /// <summary>
        /// Texts the box server ip text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private void TextBoxServerIP_TextChanged(object? sender, EventArgs e)
        {
            // Controlla se l'indirizzo IP è valido
            string[] ipSegments = textBoxServerIP.Text.Split('.');
            if (ipSegments.Length == 4 && ipSegments.All(segment => int.TryParse(segment, out int num) && num >= 0 && num <= 255))
            {
                textBoxServerIP.BackColor = Color.LightGreen;
            }
            else
            {
                textBoxServerIP.BackColor = Color.Red;
            }
            ValidateFields();
        }

        /// <summary>
        /// Texts the box text changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private void TextBox_TextChanged(object? sender, EventArgs e)
        {
            ValidateFields();
        }

        /// <summary>
        /// Validates the settings fields.
        /// </summary>
        /// <returns></returns>
        private bool ValidateFields()
        {
            bool isValid = AreTextFieldsValid() && IsServerPortValid() && IsTimeoutValid() && IsServerIPValid();
            buttonEchoTest.Enabled = isValid;
            buttonSave.Enabled = isValid;
            return isValid;
        }

        /// <summary>
        /// Checks if all text fiels are valid.
        /// </summary>
        /// <returns></returns>
        private bool AreTextFieldsValid()
        {
            return !string.IsNullOrEmpty(textBoxAETitle.Text) &&
                   !string.IsNullOrEmpty(textBoxServerIP.Text) &&
                   !string.IsNullOrEmpty(textBoxServerPort.Text) &&
                   !string.IsNullOrEmpty(textBoxTimeout.Text) &&
                   !string.IsNullOrEmpty(textBoxLocalAETitle.Text);
        }

        /// <summary>
        /// Determines whether [is server port valid].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is server port valid]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsServerPortValid()
        {
            if (!int.TryParse(textBoxServerPort.Text, out int port))
            {
                return false;
            }
            return port >= 1 && port <= 65535;
        }

        /// <summary>
        /// Determines whether [is timeout valid].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is timeout valid]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsTimeoutValid()
        {
            if (!int.TryParse(textBoxTimeout.Text, out int timeout))
            {
                return false;
            }
            return timeout >= 1;
        }

        /// <summary>
        /// Determines whether [is server ip valid].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is server ip valid]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsServerIPValid()
        {
            string[] ipSegments = textBoxServerIP.Text.Split('.');
            if (ipSegments.Length != 4)
            {
                return false;
            }
            return ipSegments.All(segment => int.TryParse(segment, out int num) && num >= 0 && num <= 255);
        }
        //private static void ShowHelpForm()
        //{
        //    UIController.ShowHelpForm();
        //    Cursor.Current = Cursors.Default;
        //}
    }
}