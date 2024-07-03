using DicomModifier.Models;
using FellowOakDicom;
using FellowOakDicom.Media;
using System.Diagnostics;

namespace DicomModifier.Controllers
{
    public class DicomFileHandler
    {
        private readonly Queue<DicomFileModel> dicomQueue;
        private string dicomDirBasePath;
        private readonly TableManager _tableManager;
        private readonly MainForm _mainForm;
        private readonly string _tempFolder;

        public DicomFileHandler(TableManager tableManager, MainForm mainForm)
        {
            dicomQueue = new Queue<DicomFileModel>();
            _tableManager = tableManager;
            _mainForm = mainForm;
            _tempFolder = Path.Combine(Path.GetTempPath(), "DicomModifier");

            if (!Directory.Exists(_tempFolder))
            {
                Directory.CreateDirectory(_tempFolder);
            }
        }

        public void AddDicomFile(string filePath)
        {
            string tempFilePath = CopyToTempFolder(filePath);
            var dicomFileModel = new DicomFileModel
            {
                FilePath = tempFilePath,
                PatientID = DicomFile.Open(tempFilePath).Dataset.GetString(DicomTag.PatientID)
            };
            dicomQueue.Enqueue(dicomFileModel);
        }

        public void AddDicomFiles(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                string tempFilePath = CopyToTempFolder(filePath);
                var dicomFileModel = new DicomFileModel
                {
                    FilePath = tempFilePath,
                    PatientID = DicomFile.Open(tempFilePath).Dataset.GetString(DicomTag.PatientID)
                };
                dicomQueue.Enqueue(dicomFileModel);
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
            var dicomFileModel = dicomQueue.Dequeue();
            return DicomFile.Open(dicomFileModel.FilePath);
        }

        public DicomDataset GetDicomData(string filePath)
        {
            return DicomFile.Open(filePath).Dataset;
        }

        private string CopyToTempFolder(string filePath)
        {
            string tempFilePath = Path.Combine(_tempFolder, Path.GetFileName(filePath));
            File.Copy(filePath, tempFilePath, true);
            return tempFilePath;
        }

        public void UpdatePatientIDInFiles(string studyInstanceUID, string newPatientID)
        {
            foreach (var dicomFileModel in dicomQueue)
            {
                var dicomFile = DicomFile.Open(dicomFileModel.FilePath);
                if (dicomFile.Dataset.GetString(DicomTag.StudyInstanceUID) == studyInstanceUID)
                {
                    dicomFile.Dataset.AddOrUpdate(DicomTag.PatientID, newPatientID);
                    dicomFile.Save(dicomFileModel.FilePath);
                }
            }
        }

        public void ResetQueue()
        {
            dicomQueue.Clear();
        }
    }
}
