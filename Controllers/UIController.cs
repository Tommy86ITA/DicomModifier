using DicomModifier;
using System.Drawing;
using System.Windows.Forms;

namespace DicomImport.Controllers
{
    public class UIController(MainForm mainForm)
    {
        private readonly MainForm _mainForm = mainForm;

        private void InvokeIfRequired(Action action)
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.Invoke(action);
            }
            else
            {
                action();
            }
        }

        public void ApplyStyles()
        {
            InvokeIfRequired(() =>
            {
                ApplyStylesToControl(_mainForm.Controls);
            });
        }

        private void ApplyStylesToControl(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (control is Button button)
                {
                    StyleButton(button);
                }
                else if (control is DataGridView dataGridView)
                {
                    StyleDataGridView(dataGridView);
                }
                else if (control.HasChildren)
                {
                    ApplyStylesToControl(control.Controls);
                }
            }
        }

        private static void StyleButton(Button button)
        {   
            if (button.Name == "buttonResetQueue") 
            {
                button.BackColor = Color.FromArgb(255, 140, 0);
            }
            else
            {
                button.BackColor = Color.FromArgb(0, 123, 255); // Blu standard per altri pulsanti
                button.Size = button.Size;
            }

            Size buttonSize = new Size(150, 40);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 10);
        }

        private static void StyleDataGridView(DataGridView dataGridView)
        {
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 45);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 123, 255);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        public void UpdateControlStates()
        {
            InvokeIfRequired(() =>
            {
                bool hasExams = _mainForm.DataGridView1.Rows.Count > 0;
                _mainForm.buttonSend.Enabled = hasExams;
                _mainForm.buttonResetQueue.Enabled = hasExams;
                _mainForm.buttonUpdateID.Enabled = hasExams;
                _mainForm.textBoxNewID.Enabled = hasExams;
            });
        }

        public void UpdateProgressBar(int value, int maximum)
        {
            InvokeIfRequired(() =>
            {
                if (_mainForm.toolStripProgressBar != null)
                {
                    _mainForm.toolStripProgressBar.Maximum = maximum;
                    _mainForm.toolStripProgressBar.Value = value;
                    _mainForm.toolStripProgressBar.Visible = true;
                }
            });
        }

        public void UpdateStatus(string status)
        {
            InvokeIfRequired(() => _mainForm.toolStripStatusLabel.Text = $"Stato: {status}");
        }

        public void UpdateFileCount(int sent, int total, string message)
        {
            InvokeIfRequired(() =>
            {
                if (total == 0)
                {
                    _mainForm.toolStripStatusLabelFileCount.Text = message;
                }
                else
                {
                    _mainForm.toolStripStatusLabelFileCount.Text = $"{message}: {sent}/{total}";
                }
            });
        }

        public void EnableControls()
        {
            InvokeIfRequired(() =>
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
            });
        }

        public void DisableControls()
        {
            InvokeIfRequired(() =>
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
            });
        }

        public void ClearTable()
        {
            InvokeIfRequired(() =>
            {
                _mainForm.dataGridView1.Rows.Clear();
                UpdateControlStates();
            });
        }

        public void ClearNewPatientIDTextBox()
        {
            InvokeIfRequired(() => _mainForm.textBoxNewID.Clear());
        }

        public void UpdateProgress(int sentFiles, int totalFiles)
        {
            InvokeIfRequired(() =>
            {
                _mainForm.UpdateFileCount(sentFiles, totalFiles, "File inviati");
                _mainForm.UpdateProgressBar(sentFiles, totalFiles);
                _mainForm.UpdateStatus("Invio in corso...");
            });
        }
    }
}