namespace DicomModifier
{
    public class MainController
    {
        private MainForm _mainForm;
        private DicomManager _dicomManager;

        public MainController(MainForm mainForm, DicomManager dicomManager)
        {
            _mainForm = mainForm;
            _dicomManager = dicomManager;

            _mainForm.OnSelectFile += MainForm_OnSelectFile;
            _mainForm.OnSelectFolder += MainForm_OnSelectFolder;
            _mainForm.OnSelectDicomDir += MainForm_OnSelectDicomDir;
            _mainForm.OnSend += MainForm_OnSend;
            _mainForm.OnResetQueue += MainForm_OnResetQueue;
            _mainForm.OnUpdatePatientID += MainForm_OnUpdatePatientID;
        }

        private void MainForm_OnSelectFile(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "DICOM files (*.dcm)|*.dcm|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _dicomManager.AddDicomFile(openFileDialog.FileName);
                    _mainForm.TableManager.AddDicomToGrid(_dicomManager.GetNextDicomFile().Dataset);
                }
            }
        }

        private void MainForm_OnSelectFolder(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    _dicomManager.AddDicomFolder(folderBrowserDialog.SelectedPath);
                    while (_dicomManager.DicomQueueCount > 0)
                    {
                        _mainForm.TableManager.AddDicomToGrid(_dicomManager.GetNextDicomFile().Dataset);
                    }
                }
            }
        }

        private void MainForm_OnSelectDicomDir(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "DICOMDIR files (DICOMDIR)|DICOMDIR|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _dicomManager.AddDicomDir(openFileDialog.FileName);
                    while (_dicomManager.DicomQueueCount > 0)
                    {
                        _mainForm.TableManager.AddDicomToGrid(_dicomManager.GetNextDicomFile().Dataset);
                    }
                }
            }
        }

        private void MainForm_OnResetQueue(object sender, EventArgs e)
        {
            _dicomManager.ResetQueue();
            _mainForm.ClearTable();
        }

        private void MainForm_OnUpdatePatientID(object sender, EventArgs e)
        {
            string newPatientID = _mainForm.GetNewPatientID();
            if (string.IsNullOrEmpty(newPatientID))
            {
                MessageBox.Show("Please enter a new Patient ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedRows = _mainForm.GetSelectedRows();
            if (selectedRows == null || selectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one exam from the table.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var row in selectedRows)
            {
                if (row.Cells["StudyInstanceUIDColumn"].Value != null)
                {
                    string studyInstanceUID = row.Cells["StudyInstanceUIDColumn"].Value.ToString();
                    _dicomManager.UpdatePatientIDInFiles(studyInstanceUID, newPatientID);
                    row.Cells["PatientIDColumn"].Value = newPatientID; // Update the table to reflect the new Patient ID
                }
            }

            MessageBox.Show("Patient ID updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_OnSend(object sender, EventArgs e)
        {
            // Implement the logic for sending DICOM files
        }
    }
}
