using DicomModifier.Models;
using FellowOakDicom;
using FellowOakDicom.Media;
using System.Diagnostics;

namespace DicomModifier.Controllers
{
    public class DicomFileHandler
    {
        private readonly Queue<string> dicomQueue;
        private string? dicomDirBasePath;                   // Dichiarato come nullable
        private readonly TableManager _tableManager;
        private readonly MainForm _mainForm;
        private readonly string _tempFolder;
        private readonly string _modifiedFolder;

        public DicomFileHandler(TableManager tableManager, MainForm mainForm)
        {
            dicomQueue = new Queue<string>();
            _tableManager = tableManager;
            _mainForm = mainForm;
            _tempFolder = Path.Combine(Path.GetTempPath(), "DicomModifier");
            _modifiedFolder = Path.Combine(_tempFolder, "modified");

            if (!Directory.Exists(_tempFolder))
            {
                Directory.CreateDirectory(_tempFolder);
            }
        }

        public string ModifiedFolder => _modifiedFolder;

        #region Funzioni di importazione

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
            List<string> dicomFiles = new();

            TraverseDirectoryRecords(dicomDir.RootDirectoryRecord, dicomFiles);

            await AddDicomFilesAsync(dicomFiles, (progress, total) =>
            {
                _mainForm.UpdateProgressBar(progress, total);
                _mainForm.UpdateFileCount(progress, total, "File elaborati");
            });
        }

        public async Task AddDicomFolderAsync(string folderPath, Action<int, int> updateProgress)
        {
            string[] dicomFiles = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories)
                .Where(file => !string.IsNullOrEmpty(Path.GetExtension(file))).ToArray();
            Debug.WriteLine($"Found {dicomFiles.Length} DICOM files in folder: {folderPath}");

            int count = 0;
            int total = dicomFiles.Length;

            foreach (string? filePath in dicomFiles)
            {
                string tempFilePath = await CopyToTempFolderAsync(filePath);
                dicomQueue.Enqueue(tempFilePath);
                count++;
                updateProgress(count, total);
            }
        }

        #endregion

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

        public int DicomQueueCount => dicomQueue.Count;

        public async Task<DicomFile?> GetNextDicomFileAsync()
        {
            if (dicomQueue.Count == 0) return null;
            string filePath = dicomQueue.Dequeue();
            return await DicomFile.OpenAsync(filePath);
        }

        public static async Task<DicomDataset> GetDicomDataAsync(string filePath)
        {
            return (await DicomFile.OpenAsync(filePath)).Dataset;
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
            List<string> updatedFilePaths = new();

            if (!Directory.Exists(_modifiedFolder))
            {
                Directory.CreateDirectory(_modifiedFolder);
            }

            string[] filePaths = Directory.GetFiles(_tempFolder);
            int count = 0;
            int total = filePaths.Length;

            for (int i = 0; i < filePaths.Length; i++)
            {
                string filePath = filePaths[i];
                try
                {
                    DicomFile dicomFile = await DicomFile.OpenAsync(filePath, FileReadOption.ReadAll);
                    if (dicomFile.Dataset.GetString(DicomTag.StudyInstanceUID) == studyInstanceUID)
                    {
                        dicomFile.Dataset.AddOrUpdate(DicomTag.PatientID, newPatientID);
                        string newFilePath = Path.Combine(_modifiedFolder, Path.GetFileName(filePath));
                        await dicomFile.SaveAsync(newFilePath);
                        updatedFilePaths.Add(newFilePath);
                    }
                    count++;
                    updateProgress(count, total);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error updating patient ID in file {filePath}: {ex.Message}");
                }
            }

            dicomQueue.Clear();
            foreach (string filePath in updatedFilePaths)
            {
                dicomQueue.Enqueue(filePath);
            }
        }

        public void ResetQueue()
        {
            dicomQueue.Clear();
        }

        public string GetModifiedFolderPath()
        {
            return _modifiedFolder;
        }
    }
}