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
        private readonly UIController _uiController;
        private CancellationTokenSource _cancellationTokenSource;

        public MainController(MainForm mainForm, DicomFileHandler dicomManager, PACSSettings settings)
        {
            _mainForm = mainForm;
            _dicomManager = dicomManager;
            _uiController = new UIController(_mainForm);
            _communicator = new PACSCommunicator(settings, _uiController);
            _tempDirectory = Path.Combine(Path.GetTempPath(), "DicomModifier");
            _cancellationTokenSource = new CancellationTokenSource();

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

        #region Event Handlers

        private async void MainForm_OnSelectFileAsync(object? sender, EventArgs e)
        {
            Logger.Log("Inizio importazione file DICOM.");
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "DICOM files (*.dcm)|*.dcm|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _uiController.UpdateStatus("Importazione in corso...");
                _uiController.UpdateProgressBar(0, 1);
                await _dicomManager.AddDicomFileAsync(openFileDialog.FileName);
                DicomFile? dicomFile = await _dicomManager.GetNextDicomFileAsync();
                if (dicomFile != null)
                {
                    _mainForm.TableManager.AddDicomToGrid(dicomFile.Dataset);
                }
                _uiController.EnableControls();
                _uiController.UpdateControlStates();
                _uiController.UpdateStatus("Importazione completata.");
                _uiController.UpdateProgressBar(1, 1);
            }
            Logger.Log("Importazione file DICOM completata.");
        }

        private async void MainForm_OnSelectFolderAsync(object? sender, EventArgs e)
        {
            Logger.Log("Inizio importazione cartella DICOM.");
            using FolderBrowserDialog folderBrowserDialog = new();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _uiController.DisableControls();
                Debug.WriteLine($"Selected folder: {folderBrowserDialog.SelectedPath}");
                _uiController.UpdateStatus("Importazione in corso...");
                List<string> files = GetFilesFromFolder(folderBrowserDialog.SelectedPath);
                InitializeProgress(files.Count);
                await ProcessFilesAsync(files);
                int fileCount = await LoadDicomFilesToGridAsync();
                FinalizeImport(fileCount);
            }
            Logger.Log("Importazione cartella DICOM completata.");
        }

        private async void MainForm_OnSelectDicomDirAsync(object? sender, EventArgs e)
        {
            Logger.Log("Inizio importazione DICOMDIR.");
            using OpenFileDialog openFileDialog = new();
            if (ConfigureOpenFileDialog(openFileDialog))
            {
                _uiController.DisableControls();
                _uiController.UpdateStatus("Importazione in corso...");
                await ImportDicomDirAsync(openFileDialog.FileName);
                int fileCount = await LoadDicomFilesToGridAsync();
                FinalizeImport(fileCount);
            }
            Logger.Log("Importazione DICOMDIR completata.");
        }

        private async void MainForm_OnUpdatePatientIDAsync(object? sender, EventArgs e)
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

            _uiController.DisableControls();
            _uiController.UpdateStatus("Aggiornamento ID Paziente in corso...");
            _uiController.UpdateProgressBar(0, selectedRows.Count);
            _uiController.UpdateFileCount(0, selectedRows.Count, "File elaborati");

            foreach (DataGridViewRow row in selectedRows)
            {
                string? studyInstanceUID = row.Cells["StudyInstanceUIDColumn"].Value?.ToString();
                if (studyInstanceUID != null)
                {
                    await _dicomManager.UpdatePatientIDInTempFolderAsync(studyInstanceUID, newPatientID, (progress, total) =>
                    {
                        _uiController.UpdateProgressBar(progress, total);
                        _uiController.UpdateFileCount(progress, total, "File elaborati");
                    });
                    row.Cells["PatientIDColumn"].Value = newPatientID; // Aggiorna l'ID visualizzato nella DataGridView
                }
            }

            _uiController.UpdateStatus("ID Paziente aggiornato con successo.");
            MessageBox.Show("ID Paziente aggiornato con successo.", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _uiController.ClearNewPatientIDTextBox();
            _uiController.UpdateControlStates();
        }

        private async void MainForm_OnSend(object? sender, EventArgs e)
        {
            try
            {
                _uiController.DisableControls();
                _mainForm.isSending = true;

                List<DataGridViewRow> selectedRows = _mainForm.GetSelectedRows();
                if (!ValidateSelectedRows(selectedRows)) return;

                List<string> filePaths = GetFilePaths();
                if (!ValidateFilePaths(filePaths)) return;

                _cancellationTokenSource = new CancellationTokenSource();

                List<string> dicomFiles = GetDicomFiles(filePaths);
                if (!ValidateDicomFiles(dicomFiles)) return;

                await SendDicomFiles(dicomFiles);

                _uiController.EnableControls();
                _uiController.UpdateControlStates();
                _mainForm.isSending = false;
            }
            catch (AggregateException ex)
            {
                HandleAggregateException(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore inaspettato: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_OnResetQueue(object? sender, EventArgs e)
        {
            _dicomManager.ResetQueue();
            MainForm.ClearTempFolder();
            _uiController.ClearTable();
            _uiController.ClearNewPatientIDTextBox();
            _uiController.UpdateControlStates();
            _uiController.UpdateProgressBar(0, 1);
            _uiController.UpdateFileCount(0, 0, "Attesa file");
            _uiController.UpdateStatus("Pronto");
        }

        public void CancelSending()
        {
            _cancellationTokenSource?.Cancel();
        }

        #endregion Event Handlers

        #region Private Methods

        private static List<string> GetFilesFromFolder(string folderPath)
        {
            return Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories).ToList();
        }

        private void InitializeProgress(int fileCount)
        {
            _uiController.UpdateProgressBar(0, fileCount);
            _uiController.UpdateFileCount(0, fileCount, "File importati");
        }

        private async Task ProcessFilesAsync(List<string> files)
        {
            int processedFiles = 0;
            foreach (string? file in files)
            {
                await _dicomManager.AddDicomFileAsync(file);
                _uiController.UpdateProgressBar(++processedFiles, files.Count);
                _uiController.UpdateFileCount(processedFiles, files.Count, "File importati");
            }
        }

        private async Task ImportDicomDirAsync(string dicomDirPath)
        {
            await _dicomManager.AddDicomDirAsync(dicomDirPath);
        }

        private async Task<int> LoadDicomFilesToGridAsync()
        {
            int fileCount = 0;
            while (_dicomManager.DicomQueueCount > 0)
            {
                DicomFile? dicomFile = await _dicomManager.GetNextDicomFileAsync();
                if (dicomFile != null)
                {
                    _mainForm.TableManager.AddDicomToGrid(dicomFile.Dataset);
                    fileCount++;
                }
            }
            return fileCount;
        }

        private void FinalizeImport(int fileCount)
        {
            _uiController.UpdateControlStates();
            _uiController.UpdateStatus("Importazione completata.");
            _uiController.UpdateProgressBar(fileCount, fileCount);
            _uiController.EnableControls();
        }

        private bool ValidateSelectedRows(List<DataGridViewRow> selectedRows)
        {
            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Per favore, seleziona almeno un esame dalla tabella.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _uiController.EnableControls();
                _mainForm.isSending = false;
                return false;
            }
            return true;
        }

        private List<string> GetFilePaths()
        {
            string modifiedFolder = _dicomManager.ModifiedFolder;
            return Directory.Exists(modifiedFolder) && Directory.GetFiles(modifiedFolder).Length != 0
                ? Directory.GetFiles(modifiedFolder).ToList()
                : Directory.GetFiles(_tempDirectory).ToList();
        }

        private bool ValidateFilePaths(List<string> filePaths)
        {
            if (filePaths.Count == 0)
            {
                MessageBox.Show("Nessun file DICOM trovato nella cartella temporanea per l'invio.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _uiController.EnableControls();
                _mainForm.isSending = false;
                return false;
            }
            return true;
        }

        private static List<string> GetDicomFiles(List<string> filePaths)
        {
            List<string> dicomFiles = [];
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
            return dicomFiles;
        }

        private bool ValidateDicomFiles(List<string> dicomFiles)
        {
            if (dicomFiles.Count == 0)
            {
                MessageBox.Show("Nessun file DICOM valido trovato per l'invio.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _uiController.EnableControls();
                _mainForm.isSending = false;
                return false;
            }
            return true;
        }

        private async Task SendDicomFiles(List<string> dicomFiles)
        {
            _mainForm.UpdateProgressBar(0, dicomFiles.Count);
            bool success = await _communicator.SendFiles(dicomFiles, _cancellationTokenSource.Token);
            if (success)
            {
                MessageBox.Show("Invio dei file riuscito!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _dicomManager.ResetQueue();
                _uiController.ClearTable();
                _uiController.ClearNewPatientIDTextBox();
                _uiController.UpdateFileCount(0, 0, "Attesa file");
                _uiController.UpdateProgressBar(0, 1);
                _uiController.UpdateStatus("Pronto");
                MainForm.ClearTempFolder();
                _uiController.UpdateControlStates();
            }
            else
            {
                MessageBox.Show("Invio dei file fallito.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void HandleAggregateException(AggregateException ex)
        {
            foreach (Exception innerException in ex.InnerExceptions)
            {
                MessageBox.Show($"Errore durante l'invio dei file: {innerException.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool ConfigureOpenFileDialog(OpenFileDialog openFileDialog)
        {
            openFileDialog.Filter = "DICOMDIR files (DICOMDIR)|DICOMDIR|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            return openFileDialog.ShowDialog() == DialogResult.OK;
        }

        #endregion Private Methods
    }
}