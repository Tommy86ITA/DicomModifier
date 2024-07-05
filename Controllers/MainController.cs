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

        // Costruttore: inizializza i componenti principali del controller
        public MainController(MainForm mainForm, DicomFileHandler dicomManager, PACSSettings settings)
        {
            _mainForm = mainForm;
            _dicomManager = dicomManager;
            _communicator = new PACSCommunicator(settings, new ProgressManager(mainForm));
            _tempDirectory = Path.Combine(Path.GetTempPath(), "DicomModifier");

            // Associa gli eventi del MainForm ai metodi di gestione
            _mainForm.OnSelectFile += MainForm_OnSelectFile;
            _mainForm.OnSelectFolder += MainForm_OnSelectFolder;
            _mainForm.OnSelectDicomDir += MainForm_OnSelectDicomDir;
            _mainForm.OnSend += MainForm_OnSend;
            _mainForm.OnResetQueue += MainForm_OnResetQueue;
            _mainForm.OnUpdatePatientID += MainForm_OnUpdatePatientID;

            // Assicura che la directory temporanea esista
            if (!Directory.Exists(_tempDirectory))
            {
                Directory.CreateDirectory(_tempDirectory);
            }
        }

        // Gestisce la selezione di un file DICOM
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
                    Debug.WriteLine($"Selected folder: {folderBrowserDialog.SelectedPath}");
                    _dicomManager.AddDicomFolder(folderBrowserDialog.SelectedPath);
                    while (_dicomManager.DicomQueueCount > 0)
                    {
                        var dicomFile = _dicomManager.GetNextDicomFile();
                        if (dicomFile != null)
                        {
                            _mainForm.TableManager.AddDicomToGrid(dicomFile.Dataset);
                        }
                        else
                        {
                            Debug.WriteLine("No more DICOM files in queue.");
                        }
                    }
                    _mainForm.UpdateControlStates();
                }
            }
        }


        // Gestisce la selezione di un file DICOMDIR
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

        // Gestisce il reset della coda di file DICOM
        private void MainForm_OnResetQueue(object sender, EventArgs e)
        {
            _dicomManager.ResetQueue();
            _mainForm.ClearTempFolder();
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

            var confirmResult = MessageBox.Show($"Sei sicuro di voler modificare l'ID Paziente in '{newPatientID}'?", "Conferma Modifica ID Paziente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmResult == DialogResult.No)
            {
                return;
            }

            foreach (var row in selectedRows)
            {
                string studyInstanceUID = row.Cells["StudyInstanceUIDColumn"].Value.ToString();
                _dicomManager.UpdatePatientIDInTempFolder(studyInstanceUID, newPatientID);
                row.Cells["PatientIDColumn"].Value = newPatientID; // Aggiorna l'ID visualizzato nella DataGridView
            }

            MessageBox.Show("ID Paziente aggiornato con successo.", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _mainForm.ClearNewPatientIDTextBox();
            _mainForm.UpdateControlStates();
        }



        // Gestisce l'invio dei file DICOM
        private async void MainForm_OnSend(object sender, EventArgs e)
        {
            try
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

                // Assicura che la directory temporanea esista prima di accedervi
                if (!Directory.Exists(_tempDirectory))
                {
                    MessageBox.Show("La cartella temporanea non esiste.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _mainForm.EnableControls();
                    _mainForm.isSending = false;
                    return;
                }

                _cancellationTokenSource = new CancellationTokenSource();

                // Cerca tutti i file nella cartella temporanea, indipendentemente dall'estensione
                var filePaths = Directory.GetFiles(_tempDirectory).ToList();
                var dicomFiles = new List<string>();

                foreach (var filePath in filePaths)
                {
                    try
                    {
                        // Prova ad aprire il file come DICOM per assicurarti che sia un file DICOM valido
                        var dicomFile = DicomFile.Open(filePath);
                        dicomFiles.Add(filePath);
                    }
                    catch
                    {
                        // Ignora il file se non può essere aperto come DICOM
                    }
                }

                if (dicomFiles.Count == 0)
                {
                    MessageBox.Show("Nessun file DICOM trovato nella cartella temporanea per l'invio.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _mainForm.EnableControls();
                    _mainForm.isSending = false;
                    return;
                }

                bool success = await _communicator.SendFiles(_cancellationTokenSource.Token);
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
                foreach (var innerException in ex.InnerExceptions)
                {
                    MessageBox.Show($"Errore durante l'invio dei file: {innerException.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore inaspettato: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // Metodo per cancellare l'invio dei file
        public void CancelSending()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
