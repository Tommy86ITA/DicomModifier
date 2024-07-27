//// Interfaces/DicomFileHandler.cs

using FellowOakDicom;
using FellowOakDicom.Media;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DicomModifier.Controllers
{
    public partial class DicomFileHandler
    {
        private readonly Queue<string> dicomQueue;
        private string? dicomDirBasePath;
        private readonly UIController _uiController;
        private readonly string _tempFolder;

        public DicomFileHandler(UIController uiController)
        {
            dicomQueue = new Queue<string>();
            _uiController = uiController;
            _tempFolder = Path.Combine(Path.GetTempPath(), "DicomImport");
            ModifiedFolder = Path.Combine(_tempFolder, "modified");

            if (!Directory.Exists(_tempFolder))
            {
                Directory.CreateDirectory(_tempFolder);
            }
        }

        public string ModifiedFolder { get; }

        #region Import methods

        public async Task AddDicomFileAsync(string filePath)
        {
            string tempFilePath = await CopyToTempFolderAsync(filePath);
            dicomQueue.Enqueue(tempFilePath);
        }

        public async Task AddDicomFilesAsync(IEnumerable<string> filePaths, Action<int, int> updateProgress)
        {
            int count = 0;
            int total = filePaths.Count();

            foreach (string filePath in filePaths)
            {
                string tempFilePath = await CopyToTempFolderAsync(filePath);
                dicomQueue.Enqueue(tempFilePath);
                count++;
                updateProgress(count, total);
            }
        }

        public async Task AddDicomDirAsync(string dicomDirPath)
        {
            dicomDirBasePath = Path.GetDirectoryName(dicomDirPath);
            Debug.WriteLine($"DICOMDIR base path: {dicomDirBasePath}");
            DicomDirectory dicomDir = await DicomDirectory.OpenAsync(dicomDirPath);
            List<string> dicomFiles = [];

            TraverseDirectoryRecords(dicomDir.RootDirectoryRecord, dicomFiles);

            await AddDicomFilesAsync(dicomFiles, (progress, total) =>
            {
                _uiController.UpdateProgressBar(progress, total);
                _uiController.UpdateFileCount(progress, total, "File elaborati");
            });
        }

        private void TraverseDirectoryRecords(DicomDirectoryRecord record, List<string> dicomFiles)
        {
            while (record != null)
            {
                if (record.Contains(DicomTag.ReferencedFileID))
                {
                    string[] fileIDs = record.GetValues<string>(DicomTag.ReferencedFileID);
                    Debug.WriteLine($"ReferencedFileID values: {string.Join(", ", fileIDs)}");

                    string combinedPath = Path.Combine(dicomDirBasePath!, Path.Combine(fileIDs));
                    combinedPath = Path.GetFullPath(combinedPath);
                    Debug.WriteLine($"Generated combined file path: {combinedPath}");

                    if (File.Exists(combinedPath))
                    {
                        Debug.WriteLine($"Adding file to queue: {combinedPath}");
                        dicomFiles.Add(combinedPath);
                    }
                    else
                    {
                        Debug.WriteLine($"File does not exist or is not accessible: {combinedPath}");
                    }
                }

                TraverseDirectoryRecords(record.LowerLevelDirectoryRecord, dicomFiles);
                record = record.NextDirectoryRecord;
            }
        }

        #endregion Import methods

        public int DicomQueueCount => dicomQueue.Count;

        public async Task<DicomFile?> GetNextDicomFileAsync()
        {
            if (dicomQueue.Count == 0) return null;
            string filePath = dicomQueue.Dequeue();
            return await DicomFile.OpenAsync(filePath);
        }

        private async Task<string> CopyToTempFolderAsync(string filePath)
        {
            string tempFilePath = Path.Combine(_tempFolder, $"{Path.GetFileNameWithoutExtension(filePath)}_{Guid.NewGuid()}{Path.GetExtension(filePath)}");

            try
            {
                // Controllo se il file di origine è accessibile in lettura
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Il file specificato non esiste.", filePath);
                }

                await using FileStream sourceStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                await using FileStream destinationStream = new(tempFilePath, FileMode.Create, FileAccess.Write);
                await sourceStream.CopyToAsync(destinationStream);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Log dell'errore o gestione appropriata
                throw new IOException($"Accesso negato durante la copia del file {filePath} nella cartella temporanea.", ex);
            }
            catch (Exception ex)
            {
                // Log dell'errore o gestione appropriata
                throw new IOException($"Errore durante la copia del file {filePath} nella cartella temporanea.", ex);
            }

            return tempFilePath;
        }

        public async Task UpdatePatientIDInTempFolderAsync(string studyInstanceUID, string newPatientID, Action<int, int> updateProgress)
        {
            List<string> updatedFilePaths = [];

            EnsureModifiedFolderExists();
            string[] filePaths = Directory.GetFiles(_tempFolder);

            await ProcessFilesAsync(filePaths, studyInstanceUID, newPatientID, updateProgress, updatedFilePaths);
            UpdateDicomQueue(updatedFilePaths);
            _uiController.EnableControls();
        }

        private void EnsureModifiedFolderExists()
        {
            if (!Directory.Exists(ModifiedFolder))
            {
                Directory.CreateDirectory(ModifiedFolder);
            }
        }

        private async Task ProcessFilesAsync(string[] filePaths, string studyInstanceUID, string newPatientID, Action<int, int> updateProgress, List<string> updatedFilePaths)
        {
            int count = 0;
            int total = filePaths.Length;

            for (int i = 0; i < filePaths.Length; i++)
            {
                string filePath = filePaths[i];
                await ProcessSingleFileAsync(filePath, studyInstanceUID, newPatientID, updatedFilePaths);
                count++;
                updateProgress(count, total);
            }

            GC.Collect(); // Suggerimento per forzare la garbage collection dopo aver processato i file
            GC.WaitForPendingFinalizers(); // Attende il completamento della garbage collection
        }

        private async Task ProcessSingleFileAsync(string filePath, string studyInstanceUID, string newPatientID, List<string> updatedFilePaths)
        {
            try
            {
                DicomFile dicomFile = await DicomFile.OpenAsync(filePath, FileReadOption.ReadAll);
                if (dicomFile.Dataset.GetString(DicomTag.StudyInstanceUID) == studyInstanceUID)
                {
                    dicomFile.Dataset.AddOrUpdate(DicomTag.PatientID, newPatientID);
                    string newFilePath = Path.Combine(ModifiedFolder, Path.GetFileName(filePath));
                    await dicomFile.SaveAsync(newFilePath);
                    updatedFilePaths.Add(newFilePath);
                }

                dicomFile.Dataset.Clear(); // Libera la memoria usata dal dataset
                dicomFile = null!; // Elimina il riferimento al file DICOM per permettere al garbage collector di liberare memoria
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating patient ID in file {filePath}: {ex.Message}");
            }
        }

        private void UpdateDicomQueue(List<string> updatedFilePaths)
        {
            dicomQueue.Clear();
            foreach (string filePath in updatedFilePaths)
            {
                dicomQueue.Enqueue(filePath);
            }
        }

        public void ResetQueue()
        {
            dicomQueue.Clear();
            GC.Collect(); // Suggerimento per forzare la garbage collection dopo aver resettato la coda
            GC.WaitForPendingFinalizers(); // Attende il completamento della garbage collection
        }

        public static bool CheckIfRemovableDrive(string filePath)
        {
            string driveLetter = Path.GetPathRoot(filePath)!;

            Debug.WriteLine($"Checking if drive {driveLetter} is removable.");

            DriveInfo driveInfo = new(driveLetter);
            bool isRemovable = driveInfo.DriveType == DriveType.Removable || driveInfo.DriveType == DriveType.CDRom;

            Debug.WriteLine($"Drive {driveLetter} is removable: {isRemovable}");

            return isRemovable;
        }

        public static void EjectDrive(string driveLetter)
        {
            try
            {
                if (string.IsNullOrEmpty(driveLetter))
                {
                    MessageBox.Show("Drive letter is null or empty.");
                    return;
                }

                // Assicurati che driveLetter sia nel formato corretto
                if (driveLetter.EndsWith(":\\"))
                {
                    driveLetter = driveLetter.TrimEnd('\\');
                }
                //else if (!driveLetter.EndsWith(":"))
                //{
                //    driveLetter += ":";
                //}

                Debug.WriteLine($"Attempting to eject drive: {driveLetter}");

                // Apri il handle al volume
                string volume = @"\\.\" + driveLetter;
                IntPtr handle = CreateFile(volume, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

                if (handle.ToInt64() == INVALID_HANDLE_VALUE)
                {
                    throw new IOException("Unable to open volume " + driveLetter);
                }

                // Esegui il comando di espulsione
                int dummy = 0;
                DeviceIoControl(handle, IOCTL_STORAGE_EJECT_MEDIA, IntPtr.Zero, 0, IntPtr.Zero, 0, ref dummy, IntPtr.Zero);

                // Chiudi il handle
                CloseHandle(handle);

                MessageBox.Show($"L'importazione è stata completata.\nRicordati di restituire il CD/DVD al Paziente!", "Impostazione completata", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'espulsione del drive: {ex.Message}");
            }
        }

        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint OPEN_EXISTING = 3;
        private const int INVALID_HANDLE_VALUE = -1;
        private const uint IOCTL_STORAGE_EJECT_MEDIA = 0x2D4808;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            IntPtr lpOutBuffer,
            uint nOutBufferSize,
            ref int lpBytesReturned,
            IntPtr lpOverlapped);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool CloseHandle(IntPtr hObject);
    }
}