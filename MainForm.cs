using System;
using System.Windows.Forms;

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

        public MainForm()
        {
            InitializeComponent();
            InitializeEvents();
            TableManager = new TableManager(DataGridView1);

            // Inizializza le impostazioni
            _settingsController = new SettingsController(this);
            _settings = _settingsController.LoadSettings();

            // Aggiorna lo stato dei pulsanti all'avvio
            UpdateButtonStates();
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

            // Eventi per aggiornare lo stato dei pulsanti
            dataGridView1.RowsAdded += (s, e) => UpdateButtonStates();
            dataGridView1.RowsRemoved += (s, e) => UpdateButtonStates();
            textBoxNewID.TextChanged += (s, e) => UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasRows = dataGridView1.Rows.Count > 0;
            bool hasNewID = !string.IsNullOrEmpty(textBoxNewID.Text);

            buttonSend.Enabled = hasRows;
            buttonUpdateID.Enabled = hasRows && hasNewID;
            buttonResetQueue.Enabled = hasRows;
        }

        private void ButtonResetQueue_Click(object sender, EventArgs e)
        {
            OnResetQueue?.Invoke(this, EventArgs.Empty);
            UpdateButtonStates();
        }

        private void ButtonDicomFile_Click(object sender, EventArgs e)
        {
            Console.WriteLine("ButtonDicomFile_Click called");
            OnSelectFile?.Invoke(this, EventArgs.Empty);
            UpdateButtonStates();
        }

        private void ButtonFolder_Click(object sender, EventArgs e)
        {
            Console.WriteLine("ButtonFolder_Click called");
            OnSelectFolder?.Invoke(this, EventArgs.Empty);
            UpdateButtonStates();
        }

        private void ButtonDicomDir_Click(object sender, EventArgs e)
        {
            Console.WriteLine("ButtonDicomDir_Click called");
            OnSelectDicomDir?.Invoke(this, EventArgs.Empty);
            UpdateButtonStates();
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
            UpdateButtonStates();
        }

        public void ClearNewPatientIDTextBox()
        {
            textBoxNewID.Clear();
            UpdateButtonStates();
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
