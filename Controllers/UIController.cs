// Interfaces/UIController.cs


// Interfaces/UIController.cs

using DicomModifier;

namespace DicomModifier.Controllers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UIController"/> class.
    /// </summary>
    /// <param name="mainForm">The main form.</param>
    public class UIController(MainForm mainForm)
    {
        private readonly MainForm _mainForm = mainForm;

        /// <summary>
        /// Updates the control states.
        /// </summary>
        public void UpdateControlStates()
        {
            bool hasExams = _mainForm.DataGridView1.Rows.Count > 0;
            _mainForm.buttonSend.Enabled = hasExams;
            _mainForm.buttonResetQueue.Enabled = hasExams;
            _mainForm.buttonUpdateID.Enabled = hasExams;
            _mainForm.textBoxNewID.Enabled = hasExams;
        }

        /// <summary>
        /// Updates the progress bar.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="maximum">The maximum.</param>
        public void UpdateProgressBar(int value, int maximum)
        {
            if (_mainForm.toolStripProgressBar != null)
            {
                _mainForm.toolStripProgressBar.Maximum = maximum;
                _mainForm.toolStripProgressBar.Value = value;
                _mainForm.toolStripProgressBar.Visible = true;
            }
        }

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="status">The status.</param>
        public void UpdateStatus(string status)
        {
            _mainForm.toolStripStatusLabel.Text = $"Stato: {status}";
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
                _mainForm.toolStripStatusLabelFileCount.Text = message;
            }
            else
            {
                _mainForm.toolStripStatusLabelFileCount.Text = $"{message}: {sent}/{total}";
            }
        }

        /// <summary>
        /// Enables the controls.
        /// </summary>
        public void EnableControls()
        {
            _mainForm.buttonDicomFile.Enabled = true;
            _mainForm.buttonFolder.Enabled = true;
            _mainForm.buttonDicomDir.Enabled = true;
            _mainForm.buttonResetQueue.Enabled = true;
            _mainForm.buttonUpdateID.Enabled = true;
            _mainForm.settingsToolStripMenuItem.Enabled = true;
            _mainForm.dataGridView1.Enabled = true;
            _mainForm.textBoxNewID.Enabled = true;
            _mainForm.buttonSend.Enabled = _mainForm.DataGridView1.Rows.Count > 0;
        }

        /// <summary>
        /// Disables the controls.
        /// </summary>
        public void DisableControls()
        {
            _mainForm.buttonDicomFile.Enabled = false;
            _mainForm.buttonFolder.Enabled = false;
            _mainForm.buttonDicomDir.Enabled = false;
            _mainForm.buttonSend.Enabled = false;
            _mainForm.buttonResetQueue.Enabled = false;
            _mainForm.buttonUpdateID.Enabled = false;
            _mainForm.settingsToolStripMenuItem.Enabled = false;
            _mainForm.dataGridView1.Enabled = false;
            _mainForm.textBoxNewID.Enabled = false;
        }

        /// <summary>
        /// Clears the Datagrid table.
        /// </summary>
        public void ClearTable()
        {
            _mainForm.dataGridView1.Rows.Clear();
            UpdateControlStates();
        }

        /// <summary>
        /// Clears the new patient identifier text box.
        /// </summary>
        public void ClearNewPatientIDTextBox()
        {
            _mainForm.textBoxNewID.Clear();
        }

        /// <summary>
        /// Updates the progress.
        /// </summary>
        /// <param name="sentFiles">The sent files.</param>
        /// <param name="totalFiles">The total files.</param>
        public void UpdateProgress(int sentFiles, int totalFiles)
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.Invoke(new Action(() =>
                {
                    _mainForm.UpdateFileCount(sentFiles, totalFiles, "File inviati");
                    _mainForm.UpdateProgressBar(sentFiles, totalFiles);
                    _mainForm.UpdateStatus("Invio in corso...");
                }));
            }
            else
            {
                _mainForm.UpdateFileCount(sentFiles, totalFiles, "File inviati");
                _mainForm.UpdateProgressBar(sentFiles, totalFiles);
                _mainForm.UpdateStatus("Invio in corso...");
            }
        }
    }
}