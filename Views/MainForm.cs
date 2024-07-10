using DicomModifier.Controllers;
using DicomModifier.Models;

namespace DicomModifier
{
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

        public bool isSending = false;                                      // Flag per controllare se ci sono trasferimenti in corso
        private bool confirmClose = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeEvents();

            _uiController = new UIController(this);
            toolTip = new ToolTip();
            InitializeTooltips();

            _uiController.UpdateControlStates();

            TableManager = new TableManager(DataGridView1, _uiController);

            // Inizializza le impostazioni
            _settingsController = new SettingsController(this);
            _settings = _settingsController.LoadSettings();


            // Gestione della chiusura del form

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

            // Gestione della chiusura del form
            this.FormClosing += MainForm_FormClosing;
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

        public void UpdateControlStates()
        {
            bool hasExams = dataGridView1.Rows.Count > 0;
            buttonSend.Enabled = hasExams;
            buttonResetQueue.Enabled = hasExams;
            buttonUpdateID.Enabled = hasExams;
            textBoxNewID.Enabled = hasExams;
        }

        private void EsciToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            MainForm_FormClosing(sender, new FormClosingEventArgs(CloseReason.UserClosing, false));
        }

        private async void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (isSending && !confirmClose)
            {
                DialogResult result = MessageBox.Show("Ci sono trasferimenti in corso. Vuoi davvero chiudere il programma?", "Trasferimenti in corso", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    confirmClose = true;
                    if (this.Tag is MainController mainController)
                    {
                        _uiController.DisableControls();
                        dataGridView1.Enabled = false;
                        mainController.CancelSending();

                        await Task.Delay(1000); // Attendere per assicurarsi che i file vengano rilasciati
                        Application.Exit(); // Richiama la chiusura dell'applicazione
                    }
                    else
                    {
                        // Gestione del caso in cui this.Tag non è un MainController
                        MessageBox.Show("Errore: il controller principale non è disponibile.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
                else if (!isSending)
                {
                    ClearTempFolder();
                    Application.Exit();
                }
            }
        }

        public static void ClearTempFolder()
        {
            string tempPath = Path.GetTempPath() ?? throw new InvalidOperationException("Unable to get temp path");
            string tempDirectory = Path.Combine(tempPath, "DicomModifier");
            if (Directory.Exists(tempDirectory))
            {
                DirectoryInfo di = new(tempDirectory);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }


        private void ButtonResetQueue_Click(object? sender, EventArgs e)
        {
            OnResetQueue?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonDicomFile_Click(object? sender, EventArgs e)
        {
            Console.WriteLine("ButtonDicomFile_Click called");
            OnSelectFile?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonFolder_Click(object? sender, EventArgs e)
        {
            Console.WriteLine("ButtonFolder_Click called");
            OnSelectFolder?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonDicomDir_Click(object? sender, EventArgs e)
        {
            Console.WriteLine("ButtonDicomDir_Click called");
            OnSelectDicomDir?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonSend_Click(object? sender, EventArgs e)
        {
            Console.WriteLine("ButtonSend_Click called");
            OnSend?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonUpdateID_Click(object? sender, EventArgs e)
        {
            OnUpdatePatientID?.Invoke(this, EventArgs.Empty);
        }

        private void AboutToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("DICOM Modifier\nDeveloped by Thomas Amaranto - 2024", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SettingsToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            using SettingsForm settingsForm = new(_settings, _settingsController);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                _settings = settingsForm.GetSettings();
                _settingsController.SaveSettings(_settings);
            }
        }

        public string GetNewPatientID()
        {
            return textBoxNewID.Text;
        }

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

        public void UpdateStatus(string status)
        {
            toolStripStatusLabel.Text = $"Stato: {status}";
        }


        public void UpdateProgressBar(int value, int maximum)
        {
            var parent = toolStripProgressBar.GetCurrentParent();
            if (parent != null && parent.InvokeRequired)
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
            List<DataGridViewRow> selectedRows = new();
            return selectedRows;
        }
    }
}
