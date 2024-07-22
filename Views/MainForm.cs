// Interfaces/MainForm.cs

using DicomImport.Controllers;
using DicomImport.Models;
using System.Diagnostics;
using System.Reflection;

namespace DicomModifier
{
    //
    public partial class MainForm : Form
    {
        /// <summary>
        /// Gets the data grid view1.
        /// </summary>
        /// <value>
        /// The data grid view1.
        /// </value>
        public DataGridView DataGridView1 => dataGridView1;

        /// <summary>
        /// Event handlers
        /// </summary>
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

        /// <summary>
        /// Checks if there is a file transfer in progress.
        /// </summary>
        public bool isSending = false;

        /// <summary>
        /// Checks if user has confirmed closing the program.
        /// </summary>
        private bool confirmClose = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            _uiController = new UIController(this);

            InitializeComponent();
            _uiController.ApplyStyles();

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = version != null ? version.ToString() : "Versione non disponibile";
            this.Text = $"DICOM Import & Edit - v. {versionString}";

            InitializeEvents();

            toolTip = new ToolTip();
            InitializeTooltips();

            _uiController.UpdateControlStates();

            TableManager = new TableManager(DataGridView1, _uiController);

            // Inizializza le impostazioni
            _settingsController = new SettingsController(this);
            _settings = _settingsController.LoadSettings();

            ClearTempFolder();
        }

        /// <summary>
        /// Initializes the events.
        /// </summary>
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

            this.FormClosing += MainForm_FormClosing;
        }

        /// <summary>
        /// Initializes the tooltips.
        /// </summary>
        private void InitializeTooltips()
        {
            toolTip.SetToolTip(buttonDicomFile, "Seleziona un file DICOM da importare.");
            toolTip.SetToolTip(buttonFolder, "Seleziona una cartella contenente file DICOM da importare.");
            toolTip.SetToolTip(buttonDicomDir, "Seleziona un file DICOMDIR da importare.");
            toolTip.SetToolTip(buttonSend, "Invia i file DICOM selezionati al server PACS.");
            toolTip.SetToolTip(buttonResetQueue, "Pulisci la coda di file DICOM.");
            toolTip.SetToolTip(buttonUpdateID, "Aggiorna l'ID paziente per i file DICOM selezionati.");
        }

        /// <summary>
        /// Manages the tool strip menu item click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void EsciToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            CloseApplication();
        }

        private void HelpToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            UIController.ShowHelp();
        }

        /// <summary>
        /// Opens a message box with information about the application.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void AboutToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = version != null ? version.ToString() : "Versione non disponibile";
            MessageBox.Show($"DICOM Import & Edit \nVersione: {versionString}\n\nDeveloped by Thomas Amaranto - 2024\n\nRilasciato sotto licenza MIT.", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Opens the Settings form.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SettingsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using SettingsForm settingsForm = new(_settings, _settingsController, _uiController);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                _settings = settingsForm.GetSettings();
                _settingsController.SaveSettings(_settings);
            }
        }

        /// <summary>
        /// Manages the form closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            CloseApplication();
        }

        /// <summary>
        /// Checks if there are transfers in progress. If yes, asks the user if they want to continue. If yes, sends a cancellation token to the server, then awaits for 1s and closes the application.
        /// Else, the program clears the temp folder and closes the application.
        /// </summary>
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

        /// <summary>
        /// Clears the temporary folder.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Unable to get temp path</exception>
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

        /// <summary>
        /// Manages the queue reset button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ButtonResetQueue_Click(object? sender, EventArgs e)
        {
            OnResetQueue?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Opens the dialog to import a single DICOM file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ButtonDicomFile_Click(object? sender, EventArgs e)
        {
            Debug.WriteLine("ButtonDicomFile_Click called");
            OnSelectFile?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Opens the dialog to import a folder with DICOM files. Folders are searched recursively.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ButtonFolder_Click(object? sender, EventArgs e)
        {
            Debug.WriteLine("ButtonFolder_Click called");
            OnSelectFolder?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Opens the dialog to import a DICOMDIR file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ButtonDicomDir_Click(object? sender, EventArgs e)
        {
            Debug.WriteLine("ButtonDicomDir_Click called");
            OnSelectDicomDir?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Manages the send button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Manages the update Patient ID button click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ButtonUpdateID_Click(object? sender, EventArgs e)
        {
            OnUpdatePatientID?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the new Patient ID.
        /// </summary>
        /// <returns></returns>
        public string GetNewPatientID()
        {
            return textBoxNewID.Text;
        }

        /// <summary>
        /// Updates the file count.
        /// </summary>
        /// <param name="sent">The sent.</param>
        /// <param name="total">The total.</param>
        /// <param name="message">The message.</param>
        public void UpdateFileCount(int sent, int total, string message)
        {
            if (total == 0)
            {
                toolStripStatusLabelFileCount.Text = message;
            }
            else
            {
                toolStripStatusLabelFileCount.Text = $"{message}: {sent}/{total}";
            }
        }

        /// <summary>
        /// Updates the status label.
        /// </summary>
        /// <param name="status">The status.</param>
        public void UpdateStatus(string status)
        {
            toolStripStatusLabel.Text = $"Stato: {status}";
        }

        /// <summary>
        /// Updates the progress bar.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="maximum">The maximum.</param>
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

        /// <summary>
        /// Gets the selected rows from the Datagrid.
        /// </summary>
        /// <returns></returns>
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