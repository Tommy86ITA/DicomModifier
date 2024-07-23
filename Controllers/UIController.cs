// Interfaces/UIController.cs

using System.Diagnostics;
using System.Reflection;

namespace DicomModifier.Controllers
{
    public class UIController(MainForm mainForm)
    {
        private readonly MainForm _mainForm = mainForm;

        // Invoke the action on the UI thread if required
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

        // Apply styles to the main form controls
        public void ApplyStyles()
        {
            InvokeIfRequired(() => ApplyStylesToControl(_mainForm.Controls));
        }

        // Apply styles to a collection of controls
        private static void ApplyStylesToControl(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control)
                {
                    case Button button:
                        StyleButton(button);
                        break;

                    case DataGridView dataGridView:
                        StyleDataGridView(dataGridView);
                        break;

                    default:
                        if (control.HasChildren)
                        {
                            ApplyStylesToControl(control.Controls);
                        }

                        break;
                }
            }
        }

        // Apply styles to a button
        private static void StyleButton(Button button)
        {
            button.BackColor = GetButtonColor(button.Name);
            button.FlatStyle = FlatStyle.Flat;
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 10);

            // Event handler to change style when button is enabled/disabled
            button.EnabledChanged += (sender, e) =>
            {
                if (sender is Button btn)
                {
                    btn.BackColor = btn.Enabled ? GetButtonColor(btn.Name) : Color.LightGray;
                }
            };
        }

        // Get the appropriate color for a button based on its name
        private static Color GetButtonColor(string buttonName)
        {
            if (buttonName == "buttonUpdateID" || buttonName == "buttonSend")
            {
                return Color.LightCoral;
            }
            else
            {
                return Color.DodgerBlue;
            }
        }

        // Apply styles to a DataGridView
        private static void StyleDataGridView(DataGridView dataGridView)
        {
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 45);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 123, 255);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        // Update the state of various controls based on the current state of the application
        public void UpdateControlStates()
        {
            InvokeIfRequired(() =>
            {
                bool hasExams = _mainForm.dataGridView1.Rows.Count > 0;
                _mainForm.buttonSend.Enabled = hasExams;
                _mainForm.buttonResetQueue.Enabled = hasExams;
                _mainForm.buttonUpdateID.Enabled = hasExams;
                _mainForm.textBoxNewID.Enabled = hasExams;
            });
        }

        // Update the progress bar with the given value and maximum
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

        // Update the status label with the given status message
        public void UpdateStatus(string status)
        {
            InvokeIfRequired(() => _mainForm.toolStripStatusLabel.Text = $"Stato: {status}");
        }

        // Update the file count label with the given counts and message
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

        // Enable all relevant controls
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
                _mainForm.logoutToolStripMenuItemLogout.Enabled = true;
                _mainForm.accountToolStripMenuItem.Enabled = true;
                _mainForm.buttonSend.Enabled = _mainForm.dataGridView1.Rows.Count > 0;
            });
        }

        // Disable all relevant controls
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
                _mainForm.logoutToolStripMenuItemLogout.Enabled = false;
                _mainForm.accountToolStripMenuItem.Enabled = false;
            });
        }

        // Clear the table and update control states
        public void ClearTable()
        {
            InvokeIfRequired(() =>
            {
                _mainForm.dataGridView1.Rows.Clear();
                UpdateControlStates();
            });
        }

        // Clear the new patient ID text box
        public void ClearNewPatientIDTextBox()
        {
            InvokeIfRequired(() => _mainForm.textBoxNewID.Clear());
        }

        // Update the progress and status based on the given counts
        public void UpdateProgress(int sentFiles, int totalFiles)
        {
            InvokeIfRequired(() =>
            {
                UpdateFileCount(sentFiles, totalFiles, "File inviati");
                UpdateProgressBar(sentFiles, totalFiles);
                UpdateStatus("Invio in corso...");
            });
        }

        public static void ShowHelp()
        {
            string? exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (exeDir == null)
            {
                MessageBox.Show("Il percorso dell'eseguibile non è stato trovato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string pdfPath = Path.Combine(exeDir, "Help", "UserGuide.pdf");

            if (File.Exists(pdfPath))
            {
                Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Il file della guida non è stato trovato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateUIBasedOnRole(string role)
        {
            InvokeIfRequired(() =>
            {
                // Nascondi o disabilita i controlli per i Technician
                if (role == "Technician")
                {
                    _mainForm.settingsToolStripMenuItem.Visible = false;
                    _mainForm.adminToolStripMenuItem.Visible=false;
                }
                else if (role == "Admin")
                {
                    // Abilita tutte le funzionalità
                    _mainForm.settingsToolStripMenuItem.Visible = true;
                    _mainForm.adminToolStripMenuItem.Visible = true;
                }
            });
        }
    }
}