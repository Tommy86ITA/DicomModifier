using FellowOakDicom;
using FellowOakDicom.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DicomModifier
{
    public class DicomManager
    {
        private Queue<string> dicomQueue;
        private HashSet<string> loadedFiles;
        private string dicomDirBasePath;
        private string currentPatientID;

        public DicomManager()
        {
            dicomQueue = new Queue<string>();
            loadedFiles = new HashSet<string>();
        }

        public event EventHandler<string> OnError;

        public void AddDicomFile(string filePath)
        {
            if (loadedFiles.Contains(filePath))
            {
                OnError?.Invoke(this, "File already loaded: " + filePath);
                return;
            }

            var dicomFile = DicomFile.Open(filePath);
            var patientID = dicomFile.Dataset.GetString(DicomTag.PatientID);
            if (currentPatientID != null && currentPatientID != patientID)
            {
                OnError?.Invoke(this, "Cannot load files from different patients.");
                return;
            }

            if (currentPatientID == null)
            {
                currentPatientID = patientID;
            }

            dicomQueue.Enqueue(filePath);
            loadedFiles.Add(filePath);
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

        public void ClearQueue()
        {
            dicomQueue.Clear();
            loadedFiles.Clear();
            currentPatientID = null;
        }
    }
}
