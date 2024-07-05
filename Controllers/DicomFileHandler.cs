using DicomModifier.Models;
using FellowOakDicom.Media;
using FellowOakDicom;
using System.Diagnostics;
using DicomModifier;

public class DicomFileHandler
{
    private readonly Queue<string> dicomQueue;
    private string dicomDirBasePath;
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

                string combinedPath = Path.Combine(dicomDirBasePath, Path.Combine(fileIDs));
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

    public void UpdatePatientIDInTempFolder(string studyInstanceUID, string newPatientID)
    {
        var updatedFilePaths = new List<string>();

        // Assicuriamoci che la cartella _modifiedFolder esista
        if (!Directory.Exists(_modifiedFolder))
        {
            Directory.CreateDirectory(_modifiedFolder);
        }

        foreach (var filePath in Directory.GetFiles(_tempFolder))
        {
            try
            {
                // Apriamo il file DICOM
                var dicomFile = DicomFile.Open(filePath, FileReadOption.ReadAll);

                // Modifichiamo l'ID paziente se lo StudyInstanceUID corrisponde
                if (dicomFile.Dataset.GetString(DicomTag.StudyInstanceUID) == studyInstanceUID)
                {
                    dicomFile.Dataset.AddOrUpdate(DicomTag.PatientID, newPatientID);

                    // Creiamo il nuovo percorso del file
                    string newFilePath = Path.Combine(_modifiedFolder, Path.GetFileName(filePath));

                    // Salviamo il file modificato
                    dicomFile.Save(newFilePath);

                    updatedFilePaths.Add(newFilePath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating patient ID in file {filePath}: {ex.Message}");
            }
        }

        // Aggiorniamo la coda con i nuovi percorsi dei file
        dicomQueue.Clear();
        foreach (var filePath in updatedFilePaths)
        {
            dicomQueue.Enqueue(filePath);
        }

        Debug.WriteLine("Updated queue with modified file paths:");
        foreach (var filePath in dicomQueue)
        {
            Debug.WriteLine(filePath);
        }
    }


    public void ResetQueue()
    {
        dicomQueue.Clear();
    }
}
