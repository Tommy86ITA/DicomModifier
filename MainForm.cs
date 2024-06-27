using FellowOakDicom;
using System;
using System.Windows.Forms;

namespace DicomModifier
{
    public partial class MainForm : Form
    {
        public event EventHandler OnSelectFile;
        public event EventHandler OnSelectFolder;
        public event EventHandler OnSelectDicomDir;
        public event EventHandler OnSend;

        public MainForm()
        {
            InitializeComponent();
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            buttonDicomFile.Click += ButtonDicomFile_Click;
            buttonFolder.Click += ButtonFolder_Click;
            buttonDicomDir.Click += ButtonDicomDir_Click;
            buttonSend.Click += ButtonSend_Click;
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
        }

        private void ButtonDicomFile_Click(object sender, EventArgs e)
        {
            OnSelectFile?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonFolder_Click(object sender, EventArgs e)
        {
            OnSelectFolder?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonDicomDir_Click(object sender, EventArgs e)
        {
            OnSelectDicomDir?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonSend_Click(object sender, EventArgs e)
        {
            OnSend?.Invoke(this, EventArgs.Empty);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("DICOM Modifier\nDeveloped by Thomas Amaranto - 2024", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public string GetNewPatientId()
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

        public void AddDicomToGrid(DicomFile dicomFile)
        {
            // Add logic to extract information from DicomFile and add to DataGridView
        }
    }
}
