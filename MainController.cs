namespace DicomModifier
{
    public class MainController
    {
        private readonly MainForm _mainForm;
        private readonly DicomManager _dicomManager;
        private readonly PACSCommunicator _communicator;
        private readonly string _tempDirectory;
        private CancellationTokenSource _cancellationTokenSource;

        public MainController(MainForm mainForm, DicomManager dicomManager, PACSSettings settings)
        {
            _mainForm = mainForm;
            _dicomManager = dicomManager;
            _communicator = new PACSCommunicator(settings, new ProgressManager(mainForm));
            _tempDirectory = Path.Combine(Path.GetTempPath(), "DicomModifier");

            _mainForm.OnSelectFile += MainForm_OnSelectFile;
            _mainForm.OnSelectFolder += MainForm_OnSelectFolder;
            _mainForm.OnSelectDicomDir += MainForm_OnSelectDicomDir;
            _mainForm.OnSend += MainForm_OnSend;
            _mainForm.OnResetQueue += MainForm_OnResetQueue;
            _mainForm.OnUpdatePatientID += MainForm_OnUpdatePatientID;

            // Ensure the temporary directory exists
            if (!Directory.Exists(_tempDirectory))
            {
                Directory.CreateDirectory(_tempDirectory);
            }
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
                    _mainForm.UpdateControlStates();
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
                    _mainForm.UpdateControlStates();
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
                    _mainForm.UpdateControlStates();
                }
            }
        }

        private void MainForm_OnResetQueue(object sender, EventArgs e)
        {
            _dicomManager.ResetQueue();
            _mainForm.ClearTable();
            _mainForm.ClearNewPatientIDTextBox();
            _mainForm.UpdateControlStates();
        }

        private void MainForm_OnUpdatePatientID(object sender, EventArgs e)
        {
            string newPatientID = _mainForm.GetNewPatientID();
            if (string.IsNullOrEmpty(newPatientID))
            {
                MessageBox.Show("Per favore, inserisci un nuovo ID Paziente.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedRows = _mainForm.GetSelectedRows();
            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Per favore, seleziona almeno un esame dalla tabella.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var row in selectedRows)
            {
                string studyInstanceUID = row.Cells["StudyInstanceUIDColumn"].Value.ToString();
                _dicomManager.UpdatePatientIDInFiles(studyInstanceUID, newPatientID);
                row.Cells["PatientIDColumn"].Value = newPatientID; // Update the displayed ID in the DataGridView
            }

            MessageBox.Show("ID Paziente aggiornato con successo.", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _mainForm.ClearNewPatientIDTextBox();
        }

        private async void MainForm_OnSend(object sender, EventArgs e)
        {
            _mainForm.DisableControls();
            _mainForm.isSending = true;

            var selectedRows = _mainForm.GetSelectedRows();
            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Per favore, seleziona almeno un esame dalla tabella.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _mainForm.EnableControls();
                _mainForm.isSending = false;
                return;
            }

            // Ensure the temporary directory exists before accessing it
            if (!Directory.Exists(_tempDirectory))
            {
                MessageBox.Show("La cartella temporanea non esiste.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _mainForm.EnableControls();
                _mainForm.isSending = false;
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var filePaths = Directory.GetFiles(_tempDirectory, "*.dcm").ToList();
            if (filePaths.Count == 0)
            {
                MessageBox.Show("Nessun file trovato nella cartella temporanea per l'invio.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _mainForm.EnableControls();
                _mainForm.isSending = false;
                return;
            }

            bool success = await _communicator.SendFiles(filePaths, _cancellationTokenSource.Token);
            if (success)
            {
                MessageBox.Show("Invio dei file riuscito!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _dicomManager.ResetQueue();
                _mainForm.ClearTable();
                _mainForm.ClearNewPatientIDTextBox();
                _mainForm.UpdateFileCount(0, 0);
                _mainForm.UpdateProgressBar(0);
                _mainForm.UpdateStatus("Pronto");
                _mainForm.ClearTempFolder();
            }
            else
            {
                MessageBox.Show("Invio dei file fallito.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _mainForm.EnableControls();
            _mainForm.isSending = false;
        }

        public void CancelSending()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}