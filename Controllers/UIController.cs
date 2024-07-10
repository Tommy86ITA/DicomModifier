using DicomModifier;

public class UIController
{
    private readonly MainForm _mainForm;

    public UIController(MainForm mainForm)
    {
        _mainForm = mainForm;
    }

    public void UpdateControlStates()
    {
        bool hasExams = _mainForm.DataGridView1.Rows.Count > 0;
        _mainForm.buttonSend.Enabled = hasExams;
        _mainForm.buttonResetQueue.Enabled = hasExams;
        _mainForm.buttonUpdateID.Enabled = hasExams;
        _mainForm.textBoxNewID.Enabled = hasExams;
    }

    public void UpdateProgressBar(int value, int maximum)
    {
        if (_mainForm.toolStripProgressBar != null)
        {
            _mainForm.toolStripProgressBar.Maximum = maximum;
            _mainForm.toolStripProgressBar.Value = value;
            _mainForm.toolStripProgressBar.Visible = true;
        }
    }

    public void UpdateStatus(string status)
    {
        _mainForm.toolStripStatusLabel.Text = $"Stato: {status}";
    }

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

    public void ClearTable()
    {
        _mainForm.dataGridView1.Rows.Clear();
        UpdateControlStates();
    }

    public void ClearNewPatientIDTextBox()
    {
        _mainForm.textBoxNewID.Clear();
    }
}