// Interfaces/MainController.cs

using DicomModifier.Models;
using DicomModifier.Services;
using FellowOakDicom;
using System.Diagnostics;

namespace DicomModifier.Controllers
{
    /// <summary>
    /// Orchestrates UI events and coordinates <see cref="DicomService"/> and <see cref="PacsService"/>
    /// in response to user actions on <see cref="MainForm"/>.
    /// </summary>
    public class MainController
    {
        private readonly MainForm _mainForm;
        private readonly DicomService _dicomService;
        private readonly PacsService _pacsService;
        private readonly UIController _uiController;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _autoEjectOpticalMedia;

        /// <summary>
        /// Initializes a new <see cref="MainController"/> and subscribes to all <see cref="MainForm"/> events.
        /// </summary>
        /// <param name="mainForm">The application main window.</param>
        /// <param name="dicomService">Service that manages DICOM import and patient ID updates.</param>
        /// <param name="pacsService">Service that handles PACS network communication.</param>
        /// <param name="autoEjectOpticalMedia">Whether to eject the optical drive after import.</param>
        public MainController(MainForm mainForm, DicomService dicomService, PacsService pacsService, bool autoEjectOpticalMedia = true)
        {
            _mainForm = mainForm;
            _dicomService = dicomService;
            _pacsService = pacsService;
            _autoEjectOpticalMedia = autoEjectOpticalMedia;
            _uiController = new UIController(_mainForm);
            _cancellationTokenSource = new CancellationTokenSource();

            _mainForm.OnSelectFile += MainForm_OnSelectFileAsync;
            _mainForm.OnSelectFolder += MainForm_OnSelectFolderAsync;
            _mainForm.OnSelectDicomDir += MainForm_OnSelectDicomDirAsync;
            _mainForm.OnSend += MainForm_OnSend;
            _mainForm.OnResetQueue += MainForm_OnResetQueue;
            _mainForm.OnUpdatePatientID += MainForm_OnUpdatePatientIDAsync;
        }

        /// <summary>Updates PACS settings at runtime (e.g. after the user saves new settings).</summary>
        public void UpdatePacsSettings(PACSSettings settings)
        {
            _pacsService.UpdateSettings(settings);
            _autoEjectOpticalMedia = settings.AutoEjectOpticalMedia;
        }

        #region Event Handlers
        /// <summary>Handles the Select File button: opens a file dialog and imports the chosen DICOM file.</summary>
        private async void MainForm_OnSelectFileAsync(object? sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "DICOM files (*.dcm)|*.dcm|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                _uiController.DisableControls();
                _uiController.UpdateStatus("Importazione in corso...");
                _uiController.UpdateProgressBar(0, 1);

                await _dicomService.AddDicomFileAsync(filePath);
                int fileCount = await LoadDicomFilesToGridAsync();
                FinalizeImport(fileCount, filePath);
            }
        }

        /// <summary>Handles the Select Folder button: imports all DICOM files found in the chosen folder.</summary>
        private async void MainForm_OnSelectFolderAsync(object? sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowserDialog = new();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = folderBrowserDialog.SelectedPath;
                _uiController.DisableControls();
                _uiController.UpdateStatus("Importazione in corso...");
                List<string> files = GetFilesFromFolder(folderPath);
                InitializeProgress(files.Count);
                await _dicomService.AddDicomFilesAsync(files, (done, total) =>
                {
                    _uiController.UpdateProgressBar(done, Math.Max(total, 1));
                    _uiController.UpdateFileCount(done, total, "File importati");
                });
                int fileCount = await LoadDicomFilesToGridAsync();
                FinalizeImport(fileCount, folderPath);
            }
        }

        /// <summary>Handles the Select DICOMDIR button: opens a DICOMDIR and imports all referenced files.</summary>
        private async void MainForm_OnSelectDicomDirAsync(object? sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            if (ConfigureOpenFileDialog(openFileDialog))
            {
                string dicomDirPath = openFileDialog.FileName;
                _uiController.DisableControls();
                _uiController.UpdateStatus("Importazione in corso...");
                await _dicomService.AddDicomDirAsync(dicomDirPath, (done, total) =>
                {
                    _uiController.UpdateProgressBar(done, Math.Max(total, 1));
                    _uiController.UpdateFileCount(done, total, "File importati");
                });
                int fileCount = await LoadDicomFilesToGridAsync();
                FinalizeImport(fileCount, dicomDirPath);
            }
        }

        /// <summary>
        /// Handles the Update Patient ID button: validates the selection, prompts for confirmation,
        /// and updates the PatientID tag in all temp files belonging to the selected studies.
        /// </summary>
        private async void MainForm_OnUpdatePatientIDAsync(object? sender, EventArgs e)
        {
            string newPatientID = _mainForm.GetNewPatientID();
            if (string.IsNullOrEmpty(newPatientID))
            {
                _mainForm.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Per favore, inserisci un nuovo ID Paziente.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
                return;
            }

            List<DataGridViewRow> selectedRows = _mainForm.GetSelectedRows();
            if (selectedRows.Count == 0)
            {
                _mainForm.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Per favore, seleziona almeno un esame dalla tabella.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
                return;
            }

            if (!VerifySamePatient(selectedRows))
            {
                _mainForm.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Gli esami selezionati appartengono a pazienti diversi. Non è possibile modificare l'ID Paziente.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
                return;
            }

            bool allIDsSame = true;
            foreach (var row in selectedRows)
            {
                string? currentPatientID = row.Cells["PatientIDColumn"].Value?.ToString();
                if (currentPatientID != newPatientID)
                {
                    allIDsSame = false;
                    break;
                }
            }

            if (allIDsSame)
            {
                _mainForm.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"L'ID paziente '{newPatientID}' è uguale all'ID attuale per tutti i file selezionati. I file non verranno modificati.", "Informazione", MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
                return;
            }

            DialogResult confirmResult = DialogResult.No;
            _mainForm.Invoke((MethodInvoker)delegate
            {
                confirmResult = MessageBox.Show($"Sei sicuro di voler modificare l'ID Paziente in '{newPatientID}'?", "Conferma Modifica ID Paziente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            });
            if (confirmResult == DialogResult.No)
            {
                return;
            }

            _mainForm.Invoke((MethodInvoker)delegate
            {
                _uiController.DisableControls();
                _uiController.UpdateStatus("Aggiornamento ID Paziente in corso...");
                _uiController.UpdateProgressBar(0, selectedRows.Count);
                _uiController.UpdateFileCount(0, selectedRows.Count, "File elaborati");
            });

            foreach (DataGridViewRow row in selectedRows)
            {
                string? studyInstanceUID = row.Cells["StudyInstanceUIDColumn"].Value?.ToString();
                string? currentPatientID = row.Cells["PatientIDColumn"].Value?.ToString();
                if (studyInstanceUID != null && currentPatientID != newPatientID)
                {
                    await _dicomService.UpdatePatientIDInTempFolderAsync(studyInstanceUID, newPatientID, (progress, total) =>
                    {
                        _mainForm.Invoke((MethodInvoker)delegate
                        {
                            _uiController.UpdateProgressBar(progress, total);
                            _uiController.UpdateFileCount(progress, total, "File elaborati");
                        });
                    });
                    _mainForm.Invoke((MethodInvoker)delegate
                    {
                        row.Cells["PatientIDColumn"].Value = newPatientID; // Aggiorna l'ID visualizzato nella DataGridView
                    });
                }
            }

            _mainForm.Invoke((MethodInvoker)delegate
            {
                _uiController.UpdateStatus("ID Paziente aggiornato con successo.");
                MessageBox.Show("ID Paziente aggiornato con successo.", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _uiController.ClearNewPatientIDTextBox();
                _uiController.UpdateControlStates();
            });
        }

        /// <summary>
        /// Handles the Send button: validates the file list and sends all DICOM files to the PACS via C-STORE.
        /// </summary>
        private async void MainForm_OnSend(object? sender, EventArgs e)
        {
            try
            {
                // Disabilita i controlli e imposta lo stato di invio sul thread dell'interfaccia utente
                _mainForm.Invoke((MethodInvoker)delegate
                {
                    _uiController.DisableControls();
                    _mainForm.isSending = true;
                });

                List<DataGridViewRow> selectedRows = _mainForm.GetSelectedRows();
                if (!ValidateSelectedRows(selectedRows)) return;

                List<string> filePaths = GetFilePaths();
                if (!ValidateFilePaths(filePaths)) return;

                _cancellationTokenSource = new CancellationTokenSource();

                List<string> dicomFiles = GetDicomFiles(filePaths);
                if (!ValidateDicomFiles(dicomFiles)) return;

                await SendDicomFiles(dicomFiles);
            }
            catch (AggregateException ex)
            {
                _mainForm.Invoke((MethodInvoker)delegate
                {
                    HandleAggregateException(ex);
                    _uiController.UpdateStatus("Invio dei file fallito.");
                });
            }
            catch (Exception ex)
            {
                _mainForm.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Errore inaspettato: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _uiController.UpdateStatus("Invio dei file fallito.");
                });
            }
            finally
            {
                // Abilita i controlli e aggiorna lo stato sul thread dell'interfaccia utente
                _mainForm.Invoke((MethodInvoker)delegate
                {
                    _uiController.EnableControls();
                    _uiController.UpdateControlStates();
                    _uiController.UpdateProgressBar(0, 1);
                    _uiController.UpdateFileCount(0, 0, "Attesa file");
                    _mainForm.isSending = false;
                });
            }
        }

        /// <summary>Handles the Reset Queue button: clears the import queue, temp folder, and grid.</summary>
        private void MainForm_OnResetQueue(object? sender, EventArgs e)
        {
            _dicomService.ResetQueue();
            MainForm.ClearTempFolder();
            _uiController.ClearTable();
            _uiController.ClearNewPatientIDTextBox();
            _uiController.EnableControls();
            _uiController.UpdateControlStates();
            _uiController.UpdateProgressBar(0, 1);
            _uiController.UpdateFileCount(0, 0, "Attesa file");
            _uiController.UpdateStatus("Pronto");
        }

        /// <summary>Requests cancellation of an in-progress C-STORE send operation.</summary>
        public void CancelSending()
        {
            _cancellationTokenSource?.Cancel();
        }

        #endregion Event Handlers

        #region Private Methods

        /// <summary>Returns all files found recursively under <paramref name="folderPath"/>.</summary>
        private static List<string> GetFilesFromFolder(string folderPath)
        {
            return [.. Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)];
        }

        /// <summary>Resets the progress bar and file-count label to zero with the given <paramref name="fileCount"/> maximum.</summary>
        private void InitializeProgress(int fileCount)
        {
            int maximum = Math.Max(fileCount, 1);
            _uiController.UpdateProgressBar(0, maximum);
            _uiController.UpdateFileCount(0, fileCount, "File importati");
        }

        /// <summary>Drains the import queue, adds each file to the grid, and returns the number of files loaded.</summary>
        private async Task<int> LoadDicomFilesToGridAsync()
        {
            int fileCount = 0;
            while (_dicomService.DicomQueueCount > 0)
            {
                DicomFile? dicomFile = await _dicomService.GetNextDicomFileAsync();
                if (dicomFile != null)
                {
                    _mainForm.TableManager.AddDicomToGrid(dicomFile.Dataset);
                    fileCount++;
                }
            }
            return fileCount;
        }

        /// <summary>Re-enables controls, updates status, and optionally ejects optical media after a successful import.</summary>
        private void FinalizeImport(int fileCount, string sourcePath)
        {
            _uiController.UpdateControlStates();
            _uiController.UpdateStatus("Importazione completata.");
            _uiController.UpdateProgressBar(fileCount, Math.Max(fileCount, 1));
            _uiController.EnableControls();

            TryAutoEjectOpticalMedia(sourcePath);
        }

        /// <summary>
        /// Ejects the optical drive if <see cref="_autoEjectOpticalMedia"/> is enabled
        /// and <paramref name="sourcePath"/> is on optical media.
        /// </summary>
        private void TryAutoEjectOpticalMedia(string sourcePath)
        {
            if (!_autoEjectOpticalMedia)
            {
                Debug.WriteLine("Automatic optical media eject is disabled.");
                return;
            }

            if (OpticalMediaService.GetOpticalDriveRoot(sourcePath) is null)
            {
                Debug.WriteLine($"Import source is not optical media: {sourcePath}");
                return;
            }

            if (OpticalMediaService.TryEject(sourcePath, out string? errorMessage))
            {
                _uiController.UpdateStatus("Importazione completata. CD/DVD espulso.");
                return;
            }

            _uiController.UpdateStatus("Importazione completata. Espulsione CD/DVD non riuscita.");
            MessageBox.Show(
                $"Importazione completata, ma non è stato possibile espellere automaticamente il CD/DVD.\n\nDettagli: {errorMessage}",
                "Espulsione CD/DVD non riuscita",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        /// <summary>Returns <see langword="true"/> if at least one row is selected; otherwise shows an error and returns <see langword="false"/>.</summary>
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

        /// <summary>
        /// Returns the list of file paths to send: the modified folder if it contains files,
        /// otherwise the main temp folder.
        /// </summary>
        private List<string> GetFilePaths()
        {
            string modifiedFolder = _dicomService.ModifiedFolder;
            string tempDirectory = Path.Combine(Path.GetTempPath(), "DicomImport");
            return Directory.Exists(modifiedFolder) && Directory.GetFiles(modifiedFolder).Length != 0
                ? [.. Directory.GetFiles(modifiedFolder)]
                : [.. Directory.GetFiles(tempDirectory)];
        }

        /// <summary>Returns <see langword="true"/> if <paramref name="filePaths"/> is non-empty; otherwise shows an error and returns <see langword="false"/>.</summary>
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

        /// <summary>Filters <paramref name="filePaths"/> to only those that can be parsed as valid DICOM files.</summary>
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

        /// <summary>Returns <see langword="true"/> if <paramref name="dicomFiles"/> is non-empty; otherwise shows an error and returns <see langword="false"/>.</summary>
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

        /// <summary>Sends <paramref name="dicomFiles"/> to the PACS and handles success/failure UI feedback.</summary>
        private async Task SendDicomFiles(List<string> dicomFiles)
        {
            _mainForm.UpdateProgressBar(0, dicomFiles.Count);
            bool success = await _pacsService.SendFiles(dicomFiles,
                (done, total) =>
                {
                    _mainForm.Invoke((MethodInvoker)delegate
                    {
                        _uiController.UpdateProgress(done, total);
                    });
                }, _cancellationTokenSource.Token);
            if (success)
            {
                MessageBox.Show("Invio dei file riuscito!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _dicomService.ResetQueue();
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

        /// <summary>Shows a <see cref="MessageBox"/> for each inner exception contained in an <see cref="AggregateException"/>.</summary>
        private static void HandleAggregateException(AggregateException ex)
        {
            foreach (Exception innerException in ex.InnerExceptions)
            {
                MessageBox.Show($"Errore durante l'invio dei file: {innerException.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Configures <paramref name="openFileDialog"/> for DICOMDIR selection and returns <see langword="true"/> if the user confirmed.</summary>
        private static bool ConfigureOpenFileDialog(OpenFileDialog openFileDialog)
        {
            openFileDialog.Filter = "DICOMDIR files (DICOMDIR)|DICOMDIR|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            return openFileDialog.ShowDialog() == DialogResult.OK;
        }

        /// <summary>
        /// Returns <see langword="true"/> when all <paramref name="rows"/> belong to the same patient
        /// (matched by name and date of birth).
        /// </summary>
        private static bool VerifySamePatient(List<DataGridViewRow> rows)
        {
            if (rows.Count < 2) return true;

            var firstRow = rows[0];
            string firstPatientName = firstRow.Cells["PatientNameColumn"].Value?.ToString() ?? string.Empty;
            string firstPatientDOB = firstRow.Cells["PatientDOBColumn"].Value?.ToString() ?? string.Empty;

            foreach (var row in rows.Skip(1))
            {
                string patientName = row.Cells["PatientNameColumn"].Value?.ToString() ?? string.Empty;
                string patientDOB = row.Cells["PatientDOBColumn"].Value?.ToString() ?? string.Empty;

                if (!string.Equals(firstPatientName, patientName, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(firstPatientDOB, patientDOB, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion Private Methods
    }
}
