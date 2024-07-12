// Interfaces/DicomFileHandler.cs

using FellowOakDicom;
using FellowOakDicom.Media;
using System.Diagnostics;

namespace DicomModifier.Controllers
{
    public class DicomFileHandler
    {
        /// <summary>
        /// The dicom queue
        /// </summary>
        private readonly Queue<string> dicomQueue;
        /// <summary>
        /// The dicom dir base path
        /// </summary>
        private string? dicomDirBasePath;                                                                   // Dichiarato come nullable
        private readonly UIController _uiController;
        /// <summary>
        /// The temporary folder
        /// </summary>
        private readonly string _tempFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="DicomFileHandler"/> class.
        /// </summary>
        /// <param name="uiController">The UI controller.</param>
        public DicomFileHandler(UIController uiController)
        {
            dicomQueue = new Queue<string>();
            _uiController = uiController;
            _tempFolder = Path.Combine(Path.GetTempPath(), "DicomModifier");
            ModifiedFolder = Path.Combine(_tempFolder, "modified");

            if (!Directory.Exists(_tempFolder))
            {
                Directory.CreateDirectory(_tempFolder);
            }
        }

        /// <summary>
        /// Gets the modified folder path.
        /// </summary>
        /// <value>
        /// The modified folder.
        /// </value>
        public string ModifiedFolder { get; }

        #region Import methods

        /// <summary>
        /// Adds the DICOM file asynchronous.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public async Task AddDicomFileAsync(string filePath)
        {
            string tempFilePath = await CopyToTempFolderAsync(filePath);
            dicomQueue.Enqueue(tempFilePath);
        }

        /// <summary>
        /// Adds the DICOM files asynchronous.
        /// </summary>
        /// <param name="filePaths">The file paths.</param>
        /// <param name="updateProgress">The update progress.</param>
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

        /// <summary>
        /// Navigates into the DICOMDIR file asynchronous.
        /// </summary>
        /// <param name="dicomDirPath">The dicom dir path.</param>
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

        /// <summary>
        /// Traverses the directory records.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="dicomFiles">The dicom files.</param>
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
        #endregion

        /// <summary>
        /// Gets the dicom queue count.
        /// </summary>
        /// <value>
        /// The dicom queue count.
        /// </value>
        public int DicomQueueCount => dicomQueue.Count;

        /// <summary>
        /// Gets the next DICOM file asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<DicomFile?> GetNextDicomFileAsync()
        {
            if (dicomQueue.Count == 0) return null;
            string filePath = dicomQueue.Dequeue();
            return await DicomFile.OpenAsync(filePath);
        }

        /// <summary>
        /// Copies to temporary folder asynchronous.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates the patient identifier in temporary folder asynchronous.
        /// </summary>
        /// <param name="studyInstanceUID">The study instance uid.</param>
        /// <param name="newPatientID">The new patient identifier.</param>
        /// <param name="updateProgress">The update progress.</param>
        public async Task UpdatePatientIDInTempFolderAsync(string studyInstanceUID, string newPatientID, Action<int, int> updateProgress)
        {
            List<string> updatedFilePaths = [];

            EnsureModifiedFolderExists();
            string[] filePaths = Directory.GetFiles(_tempFolder);

            await ProcessFilesAsync(filePaths, studyInstanceUID, newPatientID, updateProgress, updatedFilePaths);
            UpdateDicomQueue(updatedFilePaths);
        }

        /// <summary>
        /// Ensures the modified folder exists.
        /// </summary>
        private void EnsureModifiedFolderExists()
        {
            if (!Directory.Exists(ModifiedFolder))
            {
                Directory.CreateDirectory(ModifiedFolder);
            }
        }

        /// <summary>
        /// Processes the files asynchronous.
        /// </summary>
        /// <param name="filePaths">The file paths.</param>
        /// <param name="studyInstanceUID">The study instance uid.</param>
        /// <param name="newPatientID">The new patient identifier.</param>
        /// <param name="updateProgress">The update progress.</param>
        /// <param name="updatedFilePaths">The updated file paths.</param>
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
        }

        /// <summary>
        /// Processes the single file asynchronous.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="studyInstanceUID">The study instance uid.</param>
        /// <param name="newPatientID">The new patient identifier.</param>
        /// <param name="updatedFilePaths">The updated file paths.</param>
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating patient ID in file {filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the DICOM queue.
        /// </summary>
        /// <param name="updatedFilePaths">The updated file paths.</param>
        private void UpdateDicomQueue(List<string> updatedFilePaths)
        {
            dicomQueue.Clear();
            foreach (string filePath in updatedFilePaths)
            {
                dicomQueue.Enqueue(filePath);
            }
        }

        /// <summary>
        /// Resets the DICOM queue.
        /// </summary>
        public void ResetQueue()
        {
            dicomQueue.Clear();
        }
    }
}