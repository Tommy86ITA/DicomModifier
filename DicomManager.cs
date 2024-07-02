using FellowOakDicom;
using FellowOakDicom.Media;
using System.Diagnostics;

namespace DicomModifier
{
    public class DicomManager
    {
        private readonly Queue<string> dicomQueue;
        private string dicomDirBasePath;
        private readonly TableManager _tableManager;
        private readonly MainForm _mainForm;
        private readonly string _tempDirectory;

        public DicomManager(TableManager tableManager, MainForm mainForm)
        {
            dicomQueue = new Queue<string>();
            _tableManager = tableManager;
            _mainForm = mainForm;
            _tempDirectory = Path.Combine(Path.GetTempPath(), "DicomModifier");

            // Ensure the temporary directory exists
            if (!Directory.Exists(_tempDirectory))
            {
                Directory.CreateDirectory(_tempDirectory);
            }
        }

        public void AddDicomFile(string filePath)
        {
            string tempFilePath = CopyFileToTempDirectory(filePath);
            dicomQueue.Enqueue(tempFilePath);
        }

        public void AddDicomFiles(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                AddDicomFile(filePath);
            }
        }

        public void AddDicomDir(string dicomDirPath)
        {
            dicomDirBasePath = Path.GetDirectoryName(dicomDirPath);
            Debug.WriteLine($"DICOMDIR base path: {dicomDirBasePath}");
            var dicomDir = DicomDirectory.Open(dicomDirPath);
            var dicomFiles = new List<string>();

            TraverseDirectoryRecords(dicomDir.RootDirectoryRecord, dicomFiles);

            AddDicomFiles(dicomFiles);
        }

        public void AddDicomFolder(string folderPath)
        {
            var dicomFiles = Directory.GetFiles(folderPath, "*.dcm", SearchOption.AllDirectories);
            Debug.WriteLine($"Found {dicomFiles.Length} DICOM files in folder: {folderPath}");
            AddDicomFiles(dicomFiles);
        }

        private void TraverseDirectoryRecords(DicomDirectoryRecord record, List<string> dicomFiles)
        {
            while (record != null)
            {
                if (record.Contains(DicomTag.ReferencedFileID))
                {
                    var fileIDs = record.GetValues<string>(DicomTag.ReferencedFileID);
                    foreach (var fileID in fileIDs)
                    {
                        string filePath = Path.Combine(dicomDirBasePath, fileID.Replace('/', Path.DirectorySeparatorChar));
                        filePath = Path.GetFullPath(filePath);
                        Debug.WriteLine($"Generated file path: {filePath}");

                        if (File.Exists(filePath))
                        {
                            Debug.WriteLine($"Adding file to queue: {filePath}");
                            dicomFiles.Add(filePath);
                        }
                        else
                        {
                            Debug.WriteLine($"File does not exist or is not accessible: {filePath}");
                            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                            {
                                Debug.WriteLine($"Directory does not exist: {Path.GetDirectoryName(filePath)}");
                            }
                            else
                            {
                                Debug.WriteLine($"Directory exists: {Path.GetDirectoryName(filePath)}");
                            }
                        }

                        string imagesSubDirPath = Path.Combine(dicomDirBasePath, "IMAGES", fileID.Replace('/', Path.DirectorySeparatorChar));
                        imagesSubDirPath = Path.GetFullPath(imagesSubDirPath);
                        Debug.WriteLine($"Generated file path in IMAGES subdirectory: {imagesSubDirPath}");
                        if (File.Exists(imagesSubDirPath))
                        {
                            Debug.WriteLine($"Adding file to queue: {imagesSubDirPath}");
                            dicomFiles.Add(imagesSubDirPath);
                        }
                        else
                        {
                            Debug.WriteLine($"File does not exist or is not accessible: {imagesSubDirPath}");
                        }
                    }
                }

                TraverseDirectoryRecords(record.LowerLevelDirectoryRecord, dicomFiles);
                record = record.NextDirectoryRecord;
            }
        }

        public int DicomQueueCount => dicomQueue.Count;

        public DicomFile GetNextDicomFile()
        {
            if (dicomQueue.Count == 0) return null;
            string filePath = dicomQueue.Dequeue();
            return DicomFile.Open(filePath);
        }

        public DicomDataset GetDicomData(string filePath)
        {
            return DicomFile.Open(filePath).Dataset;
        }

        public void CopyFilesToTempFolder(string tempFolder)
        {
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            int totalFiles = dicomQueue.Count;
            int copiedFiles = 0;

            while (dicomQueue.Count > 0)
            {
                string filePath = dicomQueue.Dequeue();
                string destPath = Path.Combine(tempFolder, Path.GetFileName(filePath));
                File.Copy(filePath, destPath, true);

                copiedFiles++;
                int progress = (int)((double)copiedFiles / totalFiles * 100);

                _mainForm.UpdateFileCount(copiedFiles, totalFiles);
                _mainForm.UpdateProgressBar(progress);
                _mainForm.UpdateStatus($"Copia in corso... ({copiedFiles}/{totalFiles} file copiati)");
            }

            _mainForm.UpdateStatus("Copia completata.");
        }

        public void UpdatePatientIDInFiles(string studyInstanceUID, string newPatientID)
        {
            foreach (var filePath in dicomQueue)
            {
                var dicomFile = DicomFile.Open(filePath);
                if (dicomFile.Dataset.GetString(DicomTag.StudyInstanceUID) == studyInstanceUID)
                {
                    dicomFile.Dataset.AddOrUpdate(DicomTag.PatientID, newPatientID);
                    dicomFile.Save(filePath);
                }
            }
        }

        private string CopyFileToTempDirectory(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string tempFilePath = Path.Combine(_tempDirectory, fileName);
            File.Copy(filePath, tempFilePath, true);
            return tempFilePath;
        }

        private void ClearTempDirectory()
        {
            var tempFiles = Directory.GetFiles(_tempDirectory);
            foreach (var tempFile in tempFiles)
            {
                File.Delete(tempFile);
            }
        }

        public void ResetQueue()
        {
            dicomQueue.Clear();
            ClearTempDirectory();
        }
    }
}
