using DicomModifier.Models;
using FellowOakDicom;
using System.Diagnostics;

namespace DicomModifier.Controllers
{
    public class MainController
    {
        private readonly MainForm _mainForm;
        private readonly DicomFileHandler _dicomManager;
        private readonly PACSCommunicator _communicator;
        private readonly string _tempDirectory;
        private CancellationTokenSource _cancellationTokenSource;

        public MainController(MainForm mainForm, DicomFileHandler dicomManager, PACSSettings settings)
        {
            _mainForm = mainForm;
            _dicomManager = dicomManager;
            _communicator = new PACSCommunicator(settings, new ProgressManager(mainForm));
            _tempDirectory = Path.Combine(Path.GetTempPath(), "DicomModifier");

            _mainForm.OnSelectFile += MainForm_OnSelectFileAsync;
            _mainForm.OnSelectFolder += MainForm_OnSelectFolderAsync;
            _mainForm.OnSelectDicomDir += MainForm_OnSelectDicomDirAsync;
            _mainForm.OnSend += MainForm_OnSend;
            _mainForm.OnResetQueue += MainForm_OnResetQueue;
            _mainForm.OnUpdatePatientID += MainForm_OnUpdatePatientIDAsync;

            if (!Directory.Exists(_tempDirectory))
            {
                Directory.CreateDirectory(_tempDirectory);
            }
        }

        private async void MainForm_OnSelectFileAsync(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new())
            {
                openFileDialog.Filter = "DICOM files (*.dcm)|*.dcm|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _mainForm.UpdateStatus("Importazione in corso...");
                    _mainForm.UpdateProgressBar(0, 1);
                    await _dicomManager.AddDicomFileAsync(openFileDialog.FileName);
                    DicomFile dicomFile = await _dicomManager.GetNextDicomFileAsync();
                    if (dicomFile != null)
                    {
                        _mainForm.TableManager.AddDicomToGrid(dicomFile.Dataset);
                    }
                    _mainForm.EnableControls();
                    _mainForm.UpdateControlStates();
                    _mainForm.UpdateStatus("Importazione completata.");
                    _mainForm.UpdateProgressBar(1, 1);
                }
            }
        }

        private async void MainForm_OnSelectFolderAsync(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    _mainForm.DisableControls();
                    Debug.WriteLine($"Selected folder: {folderBrowserDialog.SelectedPath}");
                    _mainForm.UpdateStatus("Importazione in corso...");
                    List<string> files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.*", SearchOption.AllDirectories).ToList();
                    _mainForm.UpdateProgressBar(0, files.Count);
                    _mainForm.UpdateFileCount(0, files.Count, "File importati");

                    int processedFiles = 0;
                    foreach (string? file in files)
                    {
                        await _dicomManager.AddDicomFileAsync(file);
                        _mainForm.UpdateProgressBar(++processedFiles, files.Count);
                        _mainForm.UpdateFileCount(processedFiles, files.Count, "File importati");
                    }

                    while (_dicomManager.DicomQueueCount > 0)
                    {
                        DicomFile dicomFile = await _dicomManager.GetNextDicomFileAsync();
                        if (dicomFile != null)
                        {
                            _mainForm.TableManager.AddDicomToGrid(dicomFile.Dataset);
                        }
                        else
                        {
                            Debug.WriteLine("No more DICOM files in queue.");
                        }
                    }

                    _mainForm.EnableControls();
                    _mainForm.UpdateControlStates();
                    _mainForm.UpdateStatus("Importazione completata.");
                    _mainForm.UpdateProgressBar(files.Count, files.Count);
                }
            }
        }

        private async void MainForm_OnSelectDicomDirAsync(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new())
            {
                openFileDialog.Filter = "DICOMDIR files (DICOMDIR)|DICOMDIR|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _mainForm.DisableControls();
                    _mainForm.UpdateStatus("Importazione in corso...");
                    await _dicomManager.AddDicomDirAsync(openFileDialog.FileName);

                    while (_dicomManager.DicomQueueCount > 0)
                    {
                        DicomFile dicomFile = await _dicomManager.GetNextDicomFileAsync();
                        if (dicomFile != null)
                        {
                            _mainForm.TableManager.AddDicomToGrid(dicomFile.Dataset);
                        }
                    }
                    _mainForm.EnableControls();
                    _mainForm.UpdateControlStates();
                    _mainForm.UpdateStatus("Importazione completata.");
                }
            }
        }

        private async void MainForm_OnUpdatePatientIDAsync(object sender, EventArgs e)
        {
            string newPatientID = _mainForm.GetNewPatientID();
            if (string.IsNullOrEmpty(newPatientID))
            {
                MessageBox.Show("Per favore, inserisci un nuovo ID Paziente.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<DataGridViewRow> selectedRows = _mainForm.GetSelectedRows();
            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Per favore, seleziona almeno un esame dalla tabella.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult confirmResult = MessageBox.Show($"Sei sicuro di voler modificare l'ID Paziente in '{newPatientID}'?", "Conferma Modifica ID Paziente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.No)
            {
                return;
            }

            _mainForm.DisableControls();
            _mainForm.UpdateStatus("Aggiornamento ID Paziente in corso...");
            _mainForm.UpdateProgressBar(0, selectedRows.Count);
            _mainForm.UpdateFileCount(0, selectedRows.Count, "File elaborati");

            foreach (DataGridViewRow row in selectedRows)
            {
                string studyInstanceUID = row.Cells["StudyInstanceUIDColumn"].Value.ToString();
                await _dicomManager.UpdatePatientIDInTempFolderAsync(studyInstanceUID, newPatientID, (progress, total) =>
                {
                    _mainForm.UpdateProgressBar(progress, total);
                    _mainForm.UpdateFileCount(progress, total, "File elaborati");
                });
                row.Cells["PatientIDColumn"].Value = newPatientID; // Aggiorna l'ID visualizzato nella DataGridView
            }

            _mainForm.UpdateStatus("ID Paziente aggiornato con successo.");
            MessageBox.Show("ID Paziente aggiornato con successo.", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _mainForm.ClearNewPatientIDTextBox();
            _mainForm.UpdateControlStates();
        }


        private async void MainForm_OnSend(object sender, EventArgs e)
        {
            try
            {
                _mainForm.DisableControls();
                _mainForm.isSending = true;

                List<DataGridViewRow> selectedRows = _mainForm.GetSelectedRows();
                if (selectedRows.Count == 0)
                {
                    MessageBox.Show("Per favore, seleziona almeno un esame dalla tabella.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _mainForm.EnableControls();
                    _mainForm.isSending = false;
                    return;
                }

                // Controlla se esiste la cartella "modified"
                string modifiedFolder = _dicomManager.ModifiedFolder;
                List<string> filePaths = Directory.Exists(modifiedFolder) && Directory.GetFiles(modifiedFolder).Any()
                    ? Directory.GetFiles(modifiedFolder).ToList()
                    : Directory.GetFiles(_tempDirectory).ToList();

                if (!filePaths.Any())
                {
                    MessageBox.Show("Nessun file DICOM trovato nella cartella temporanea per l'invio.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _mainForm.EnableControls();
                    _mainForm.isSending = false;
                    return;
                }

                _cancellationTokenSource = new CancellationTokenSource();

                List<string> dicomFiles = new();
                foreach (string? filePath in filePaths)
                {
                    try
                    {
                        DicomFile dicomFile = DicomFile.Open(filePath);
                        dicomFiles.Add(filePath);
                    }
                    catch
                    {
                        // Ignora il file se non può essere aperto come DICOM
                    }
                }

                if (dicomFiles.Count == 0)
                {
                    MessageBox.Show("Nessun file DICOM valido trovato per l'invio.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _mainForm.EnableControls();
                    _mainForm.isSending = false;
                    return;
                }

                _mainForm.UpdateProgressBar(0, dicomFiles.Count);
                bool success = await _communicator.SendFiles(dicomFiles, _cancellationTokenSource.Token);
                if (success)
                {
                    MessageBox.Show("Invio dei file riuscito!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _dicomManager.ResetQueue();
                    _mainForm.ClearTable();
                    _mainForm.ClearNewPatientIDTextBox();
                    _mainForm.UpdateFileCount(0, 0, "Attesa file");
                    _mainForm.UpdateProgressBar(0, 1);
                    _mainForm.UpdateStatus("Pronto");
                    MainForm.ClearTempFolder();
                    _mainForm.UpdateControlStates();
                }
                else
                {
                    MessageBox.Show("Invio dei file fallito.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                _mainForm.EnableControls();
                _mainForm.UpdateControlStates();
                _mainForm.isSending = false;
            }
            catch (AggregateException ex)
            {
                foreach (Exception innerException in ex.InnerExceptions)
                {
                    MessageBox.Show($"Errore durante l'invio dei file: {innerException.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore inaspettato: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void CancelSending()
        {
            _cancellationTokenSource?.Cancel();
        }

        private void MainForm_OnResetQueue(object sender, EventArgs e)
        {
            _dicomManager.ResetQueue();
            MainForm.ClearTempFolder();
            _mainForm.ClearTable();
            _mainForm.ClearNewPatientIDTextBox();
            _mainForm.UpdateControlStates();
            _mainForm.UpdateProgressBar(0, 1);
            _mainForm.UpdateFileCount(0, 0, "Attesa file");
            _mainForm.UpdateStatus("Pronto");

        }
    }
}
