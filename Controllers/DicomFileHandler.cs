using DicomModifier.Models;
using DicomModifier;
using FellowOakDicom.Media;
using FellowOakDicom;
using System.Diagnostics;

public class DicomFileHandler
{
    private readonly Queue<string> dicomQueue;
    private string dicomDirBasePath;
    private readonly TableManager _tableManager;
    private readonly MainForm _mainForm;
    private readonly string _tempFolder;

    public DicomFileHandler(TableManager tableManager, MainForm mainForm)
    {
        dicomQueue = new Queue<string>();
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
        dicomQueue.Enqueue(tempFilePath);
    }

    public void AddDicomFiles(IEnumerable<string> filePaths)
    {
        foreach (var filePath in filePaths)
        {
            string tempFilePath = CopyToTempFolder(filePath);
            dicomQueue.Enqueue(tempFilePath);
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
        var dicomFiles = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories)
            .Where(file => !string.IsNullOrEmpty(Path.GetExtension(file))).ToArray();
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
                Debug.WriteLine($"ReferencedFileID values: {string.Join(", ", fileIDs)}");  // Log per verificare i valori estratti

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
                        // Prova a trovare il file in tutte le sottodirectory
                        var subDirFiles = Directory.GetFiles(dicomDirBasePath, Path.GetFileName(filePath), SearchOption.AllDirectories);
                        foreach (var subDirFile in subDirFiles)
                        {
                            Debug.WriteLine($"Found file in subdirectory: {subDirFile}");
                            dicomFiles.Add(subDirFile);
                        }
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

    private string CopyToTempFolder(string filePath)
    {
        string tempFilePath = Path.Combine(_tempFolder, $"{Path.GetFileNameWithoutExtension(filePath)}_{Guid.NewGuid()}{Path.GetExtension(filePath)}");
        File.Copy(filePath, tempFilePath, true);
        return tempFilePath;
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

    public void ResetQueue()
    {
        dicomQueue.Clear();
    }
}
