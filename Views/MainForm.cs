// Interfaces/MainForm.cs

using DicomModifier.Controllers;
using DicomModifier.Models;
using DicomModifier.Services;
using DicomModifier.Views;
using System.Diagnostics;
using System.Reflection;

namespace DicomModifier
{
    //
    public partial class MainForm : Form
    {
        public DataGridView DataGridView1 => dataGridView1;

        public event EventHandler? OnSelectFile;

        public event EventHandler? OnSelectFolder;

        public event EventHandler? OnSelectDicomDir;

        public event EventHandler? OnSend;

        public event EventHandler? OnResetQueue;

        public event EventHandler? OnUpdatePatientID;

        public TableManager TableManager { get; private set; }
        private readonly SettingsController _settingsController;
        private readonly UIController _uiController;
        private readonly ToolTip toolTip;
        private PACSSettings _settings;
        private readonly AuthenticationService authService;

        public bool isSending = false;

        private bool confirmClose = false;

        public MainForm(AuthenticationService authService)
        {
            _uiController = new UIController(this);
            this.authService = authService;
            InitializeComponent();
            _uiController.ApplyStyles();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = version != null ? version.ToString() : "Versione non disponibile";
            this.Text = $"DICOM Import & Edit - v. {versionString}";

            toolStripDropDownButtonUser.Text = $"Utente: {authService.CurrentUser.Username};    Ruolo: {authService.CurrentUser.Role}";

            InitializeEvents();

            toolTip = new ToolTip();
            InitializeTooltips();

            _uiController.UpdateControlStates();

            TableManager = new TableManager(DataGridView1, _uiController);

            // Inizializza le impostazioni
            _settingsController = new SettingsController(this);
            _settings = _settingsController.LoadSettings();
            _uiController.UpdateUIBasedOnRole(authService.CurrentUser.Role);
            ClearTempFolder();
        }

        private void InitializeEvents()
        {
            buttonDicomFile.Click += ButtonDicomFile_Click;
            buttonFolder.Click += ButtonFolder_Click;
            buttonDicomDir.Click += ButtonDicomDir_Click;
            buttonSend.Click += ButtonSend_Click;
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            settingsToolStripMenuItem.Click += SettingsToolStripMenuItem_Click;
            buttonResetQueue.Click += ButtonResetQueue_Click;
            buttonUpdateID.Click += ButtonUpdateID_Click;
            esciToolStripMenuItem.Click += EsciToolStripMenuItem_Click;
            helpToolStripMenuItem.Click += HelpToolStripMenuItem_Click;
            accountToolStripMenuItem.Click += AccountOptionsToolStripMenuItem_Click;

            logoutToolStripMenuItemLogout.Click += ToolStripMenuItemLogout_Click;

            manageUserToolStripMenuItem.Click += ManageUserToolStripMenuItem_Click;

            this.FormClosing += MainForm_FormClosing;
        }

        private void ManageUserToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using ManageUsersForm manageUsersForm = new(authService);  // Passa authService qui
            manageUsersForm.ShowDialog();
        }

        private void InitializeTooltips()
        {
            toolTip.SetToolTip(buttonDicomFile, "Seleziona un file DICOM da importare.");
            toolTip.SetToolTip(buttonFolder, "Seleziona una cartella contenente file DICOM da importare.");
            toolTip.SetToolTip(buttonDicomDir, "Seleziona un file DICOMDIR da importare.");
            toolTip.SetToolTip(buttonSend, "Invia i file DICOM selezionati al server PACS.");
            toolTip.SetToolTip(buttonResetQueue, "Pulisci la coda di file DICOM.");
            toolTip.SetToolTip(buttonUpdateID, "Aggiorna l'ID paziente per i file DICOM selezionati.");
        }

        private void EsciToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            CloseApplication();
        }

        private void HelpToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            UIController.ShowHelp();
        }

        private void AboutToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = version != null ? version.ToString() : "Versione non disponibile";
            MessageBox.Show($"DICOM Import & Edit \nVersione: {versionString}\n\nDeveloped by Thomas Amaranto - 2024\n\nRilasciato sotto licenza MIT.", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SettingsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using SettingsForm settingsForm = new(_settings, _settingsController, _uiController);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                _settings = settingsForm.GetSettings();
                _settingsController.SaveSettings(_settings);
            }
        }

        private void ToolStripMenuItemLogout_Click(object? sender, EventArgs e)
        {
            this.Hide();
            ClearTempFolder();
            using LoginForm loginForm = new(authService);
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                this.Show();
                // Riaggiorna l'interfaccia utente in base al nuovo login
                _uiController.UpdateUIBasedOnRole(authService.CurrentUser.Role);
                logoutToolStripMenuItemLogout.Text = $"Utente: {authService.CurrentUser.Username};  Ruolo: {authService.CurrentUser.Role}";
            }
            else
            {
                CloseApplication();
            }
        }

        private void AccountOptionsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using ChangePasswordForm changePasswordForm = new(authService);
            changePasswordForm.ShowDialog();
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            CloseApplication();
        }

        private void CloseApplication()
        {
            Debug.WriteLine("CloseApplication method called.");

            if (isSending && !confirmClose)
            {
                Debug.WriteLine("Transfers are in progress and confirmation is not given.");

                DialogResult result = MessageBox.Show("Ci sono trasferimenti in corso. Vuoi davvero chiudere il programma?", "Trasferimenti in corso", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    Debug.WriteLine("User confirmed to close the application.");

                    confirmClose = true;
                    MainController mainController = (MainController)this.Tag!;
                    _uiController.DisableControls();
                    dataGridView1.Enabled = false;
                    mainController.CancelSending();

                    Task.Delay(1000).Wait(); // Attendere per assicurarsi che i file vengano rilasciati
                    Application.Exit(); // Richiama la chiusura dell'applicazione
                }
                else
                {
                    Debug.WriteLine("User canceled the close operation.");
                }
            }
            else
            {
                Debug.WriteLine("No transfers in progress or confirmation already given.");
                ClearTempFolder();
                Application.Exit();
            }
        }

        public static void ClearTempFolder()
        {
            string tempPath = Path.GetTempPath() ?? throw new InvalidOperationException("Unable to get temp path");
            string tempDirectory = Path.Combine(tempPath, "DicomImport");
            if (Directory.Exists(tempDirectory))
            {
                DirectoryInfo di = new(tempDirectory);
                try
                {
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        private void ButtonResetQueue_Click(object? sender, EventArgs e)
        {
            OnResetQueue?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonDicomFile_Click(object? sender, EventArgs e)
        {
            Debug.WriteLine("ButtonDicomFile_Click called");
            OnSelectFile?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonFolder_Click(object? sender, EventArgs e)
        {
            Debug.WriteLine("ButtonFolder_Click called");
            OnSelectFolder?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonDicomDir_Click(object? sender, EventArgs e)
        {
            Debug.WriteLine("ButtonDicomDir_Click called");
            OnSelectDicomDir?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonSend_Click(object? sender, EventArgs e)
        {
            Debug.WriteLine("ButtonSend_Click called");
            var confirmResult = MessageBox.Show("Sei sicuro di voler inviare i file al PACS?", "Conferma invio", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            // Se l'utente seleziona 'No', interrompi l'operazione
            if (confirmResult == DialogResult.Yes)
            {
                OnSend?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ButtonUpdateID_Click(object? sender, EventArgs e)
        {
            OnUpdatePatientID?.Invoke(this, EventArgs.Empty);
        }

        public string GetNewPatientID()
        {
            return textBoxNewID.Text;
        }

        public void UpdateStatus(string status)
        {
            toolStripStatusLabel.Text = $"Stato: {status}";
        }

        public void UpdateProgressBar(int value, int maximum)
        {
            var parent = toolStripProgressBar.GetCurrentParent();
            if (parent?.InvokeRequired == true)
            {
                parent.Invoke(new Action<int, int>(UpdateProgressBar), value, maximum);
            }
            else
            {
                toolStripProgressBar.Maximum = maximum;
                toolStripProgressBar.Value = value;
            }
        }

        public List<DataGridViewRow> GetSelectedRows()
        {
            List<DataGridViewRow> selectedRows = [];
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                selectedRows.Add(row);
                Debug.WriteLine($"Selected row: {row.Index}, PatientID: {row.Cells["PatientIDColumn"].Value}");
            }
            return selectedRows;
        }
    }
}