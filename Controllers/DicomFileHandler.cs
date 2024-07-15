//// Interfaces/DicomFileHandler.cs

using FellowOakDicom;
using FellowOakDicom.Media;
using System.Diagnostics;

namespace DicomImport.Controllers
{
    public class DicomFileHandler
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
            await using (FileStream sourceStream = File.Open(filePath, FileMode.Open))
            await using (FileStream destinationStream = File.Create(tempFilePath))
            {
                await sourceStream.CopyToAsync(destinationStream);
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
    }
}