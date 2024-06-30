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

        public MainForm()
        {
            InitializeComponent();
            InitializeEvents();
            TableManager = new TableManager(DataGridView1);
        }

        private void InitializeEvents()
        {
            buttonDicomFile.Click += ButtonDicomFile_Click;
            buttonFolder.Click += ButtonFolder_Click;
            buttonDicomDir.Click += ButtonDicomDir_Click;
            buttonSend.Click += ButtonSend_Click;
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            buttonResetQueue.Click += ButtonResetQueue_Click;
            buttonUpdateID.Click += ButtonUpdateID_Click;
            impostazioniToolStripMenuItem.Click += ImpostazioniToolStripMenuItem_Click;
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

        private void ImpostazioniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingsForm settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog();
            }
        }

            private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("DICOM Modifier\nDeveloped by Thomas Amaranto - 2024", "Informazioni", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public string GetNewPatientID()
        {
            return string.IsNullOrWhiteSpace(textBoxNewID.Text) ? null : textBoxNewID.Text;
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
