// Interfaces/UIController.cs

using System.Diagnostics;
using System.Reflection;

namespace DicomModifier.Controllers
{
    /// <summary>
    /// Provides thread-safe helper methods to update the <see cref="MainForm"/> UI:
    /// control states, progress bar, status labels, and visual styles.
    /// </summary>
    public class UIController(MainForm mainForm)
    {
        private readonly MainForm _mainForm = mainForm;

        /// <summary>Executes <paramref name="action"/> on the UI thread, marshalling if required.</summary>
        private void InvokeIfRequired(Action action)
        {
            if (_mainForm.InvokeRequired)
                _mainForm.Invoke(action);
            else
                action();
        }

        /// <summary>Applies visual styles (flat buttons, grid header colours) to all form controls.</summary>
        public void ApplyStyles()
        {
            InvokeIfRequired(() => ApplyStylesToControl(_mainForm.Controls));
        }

        /// <summary>Recursively applies visual styles to a collection of controls.</summary>
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

        /// <summary>Applies flat-style colours and an enabled/disabled handler to a button.</summary>
        private static void StyleButton(Button button)
        {
            button.BackColor = GetButtonColor(button.Name);
            button.FlatStyle = FlatStyle.Flat;
            button.ForeColor = Color.White;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 10);

            // Change colour when the button is enabled or disabled.
            button.EnabledChanged += (sender, e) =>
            {
                if (sender is Button btn)
                {
                    btn.BackColor = btn.Enabled ? GetButtonColor(btn.Name) : Color.LightGray;
                }
            };
        }

        /// <summary>
        /// Returns the accent colour for a button based on its name.
        /// Action buttons (Send, UpdateID) use red; all others use blue.
        /// </summary>
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

        /// <summary>Applies dark header and selection colours to a <see cref="DataGridView"/>.</summary>
        private static void StyleDataGridView(DataGridView dataGridView)
        {
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 45);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 123, 255);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        /// <summary>
        /// Enables or disables the Send, Reset, UpdateID, and NewID controls
        /// based on whether the grid contains any rows.
        /// </summary>
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

        /// <summary>Sets the progress bar to the given <paramref name="value"/> out of <paramref name="maximum"/>.</summary>
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

        /// <summary>Updates the status strip label with the given <paramref name="status"/> message.</summary>
        public void UpdateStatus(string status)
        {
            InvokeIfRequired(() => _mainForm.toolStripStatusLabel.Text = $"Stato: {status}");
        }

        /// <summary>
        /// Updates the file-count label.
        /// When <paramref name="total"/> is zero only <paramref name="message"/> is shown;
        /// otherwise the format is <c>{message}: {sent}/{total}</c>.
        /// </summary>
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

        /// <summary>Re-enables all import, send, reset, and settings controls.</summary>
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
                _mainForm.buttonSend.Enabled = _mainForm.dataGridView1.Rows.Count > 0;
            });
        }

        /// <summary>Disables all import, send, reset, and settings controls during a long-running operation.</summary>
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

        /// <summary>Clears all grid rows and refreshes control states.</summary>
        public void ClearTable()
        {
            InvokeIfRequired(() =>
            {
                _mainForm.dataGridView1.Rows.Clear();
                UpdateControlStates();
            });
        }

        /// <summary>Clears the New Patient ID text box.</summary>
        public void ClearNewPatientIDTextBox()
        {
            InvokeIfRequired(() => _mainForm.textBoxNewID.Clear());
        }

        /// <summary>
        /// Updates the file count, progress bar, and status label simultaneously
        /// during a C-STORE send operation.
        /// </summary>
        public void UpdateProgress(int sentFiles, int totalFiles)
        {
            InvokeIfRequired(() =>
            {
                UpdateFileCount(sentFiles, totalFiles, "File inviati");
                UpdateProgressBar(sentFiles, totalFiles);
                UpdateStatus("Invio in corso...");
            });
        }

        /// <summary>Opens the user guide PDF from the application's <c>Help</c> sub-directory.</summary>
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
    }
}
