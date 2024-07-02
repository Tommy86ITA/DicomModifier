namespace DicomModifier
{
    public partial class MainForm : Form
    {
        public DataGridView DataGridView1 => dataGridView1;

        public event EventHandler OnSelectFile;
        public event EventHandler OnSelectFolder;
        public event EventHandler OnSelectDicomDir;
        public event EventHandler OnSend;
        public event EventHandler OnResetQueue;
        public event EventHandler OnUpdatePatientID;

        public TableManager TableManager { get; private set; }
        private readonly SettingsController _settingsController;
        private PACSSettings _settings;
        public bool isSending = false; // Flag per controllare se ci sono trasferimenti in corso

        public MainForm()
        {
            InitializeComponent();
            InitializeEvents();
            UpdateControlStates();
            TableManager = new TableManager(DataGridView1);

            // Inizializza le impostazioni
            _settingsController = new SettingsController(this);
            _settings = _settingsController.LoadSettings();

            // Gestione della chiusura del form
            this.FormClosing += MainForm_FormClosing;
            ClearTempFolder();
        }

        private void InitializeEvents()
        {
            buttonDicomFile.Click += ButtonDicomFile_Click;
            buttonFolder.Click += ButtonFolder_Click;
            buttonDicomDir.Click += ButtonDicomDir_Click;
            buttonSend.Click += ButtonSend_Click;
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            buttonResetQueue.Click += ButtonResetQueue_Click;
            buttonUpdateID.Click += ButtonUpdateID_Click;
            esciToolStripMenuItem.Click += EsciToolStripMenuItem_Click;
        }

        public void DisableControls()
        {
            buttonDicomFile.Enabled = false;
            buttonFolder.Enabled = false;
            buttonDicomDir.Enabled = false;
            buttonSend.Enabled = false;
            buttonResetQueue.Enabled = false;
            buttonUpdateID.Enabled = false;
            settingsToolStripMenuItem.Enabled = false;
            dataGridView1.Enabled = false;
            textBoxNewID.Enabled = false;
        }

        public void EnableControls()
        {
            buttonDicomFile.Enabled = true;
            buttonFolder.Enabled = true;
            buttonDicomDir.Enabled = true;
            buttonResetQueue.Enabled = true;
            buttonUpdateID.Enabled = true;
            settingsToolStripMenuItem.Enabled = true;
            dataGridView1.Enabled = true;
            textBoxNewID.Enabled = true;
            // buttonSend remains disabled until files are loaded
        }

        public void UpdateControlStates()
        {
            bool hasExams = dataGridView1.Rows.Count > 0;
            buttonSend.Enabled = hasExams;
            buttonResetQueue.Enabled = hasExams;
            buttonUpdateID.Enabled = hasExams;
            textBoxNewID.Enabled = hasExams;
        }

        private void EsciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm_FormClosing(sender, new FormClosingEventArgs(CloseReason.UserClosing, false));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isSending)
            {
                var result = MessageBox.Show("Ci sono trasferimenti in corso. Vuoi davvero chiudere il programma?", "Trasferimenti in corso", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    var mainController = (MainController)this.Tag;
                    mainController.CancelSending();
                    isSending = false; // Interrompi il trasferimento
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                ClearTempFolder();
            }
        }

        public void ClearTempFolder()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), "DicomModifier");
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        private void ButtonResetQueue_Click(object sender, EventArgs e)
        {
            OnResetQueue?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonDicomFile_Click(object sender, EventArgs e)
        {
            Console.WriteLine("ButtonDicomFile_Click called");
            OnSelectFile?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonFolder_Click(object sender, EventArgs e)
        {
            Console.WriteLine("ButtonFolder_Click called");
            OnSelectFolder?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonDicomDir_Click(object sender, EventArgs e)
        {
            Console.WriteLine("ButtonDicomDir_Click called");
            OnSelectDicomDir?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonSend_Click(object sender, EventArgs e)
        {
            Console.WriteLine("ButtonSend_Click called");
            OnSend?.Invoke(this, EventArgs.Empty);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("DICOM Modifier\nDeveloped by Thomas Amaranto - 2024", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm(_settings, _settingsController))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    _settings = settingsForm.GetSettings();
                    _settingsController.SaveSettings(_settings);
                }
            }
        }

        public string GetNewPatientID()
        {
            return textBoxNewID.Text;
        }

        public void UpdateStatus(string status)
        {
            toolStripStatusLabel.Text = $"Stato: {status}";
        }

        public void UpdateFileCount(int sent, int total)
        {
            toolStripStatusLabelFileCount.Text = $"File inviati: {sent}/{total}";
        }

        public void UpdateProgressBar(int value)
        {
            toolStripProgressBar.Value = value;
        }

        public void ClearTable()
        {
            dataGridView1.Rows.Clear();
            UpdateControlStates();
        }

        public void ClearNewPatientIDTextBox()
        {
            textBoxNewID.Clear();
        }

        public List<DataGridViewRow> GetSelectedRows()
        {
            List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                selectedRows.Add(row);
            }
            return selectedRows;
        }

        private void ButtonUpdateID_Click(object sender, EventArgs e)
        {
            OnUpdatePatientID?.Invoke(this, EventArgs.Empty);
        }

    }
}
