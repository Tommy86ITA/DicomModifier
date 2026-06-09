using FellowOakDicom;
using FellowOakDicom.Media;
using System.Diagnostics;

namespace DicomModifier.Services;

/// <summary>
/// Handles all DICOM file operations: import, queue management, and patient ID update.
/// Has no dependency on UI components.
/// </summary>
public class DicomService
{
    private readonly Queue<string> _dicomQueue;
    private string? _dicomDirBasePath;
    private readonly string _tempFolder;

    /// <summary>
    /// Initializes a new <see cref="DicomService"/>, creating the temp and modified folders if necessary.
    /// </summary>
    public DicomService()
    {
        _dicomQueue = new Queue<string>();
        _tempFolder = Path.Combine(Path.GetTempPath(), "DicomImport");
        ModifiedFolder = Path.Combine(_tempFolder, "modified");

        if (!Directory.Exists(_tempFolder))
            Directory.CreateDirectory(_tempFolder);
    }

    /// <summary>Gets the folder where modified DICOM files are stored.</summary>
    public string ModifiedFolder { get; }

    /// <summary>Gets the number of files waiting in the import queue.</summary>
    public int DicomQueueCount => _dicomQueue.Count;

    #region Import

    /// <summary>Copies a single DICOM file to the temp folder and enqueues it.</summary>
    public async Task AddDicomFileAsync(string filePath)
    {
        string tempFilePath = await CopyToTempFolderAsync(filePath);
        _dicomQueue.Enqueue(tempFilePath);
    }

    /// <summary>Copies multiple DICOM files to the temp folder, reporting progress via callback.</summary>
    public async Task AddDicomFilesAsync(IEnumerable<string> filePaths, Action<int, int> updateProgress)
    {
        int count = 0;
        int total = filePaths.Count();
        foreach (string filePath in filePaths)
        {
            string tempFilePath = await CopyToTempFolderAsync(filePath);
            _dicomQueue.Enqueue(tempFilePath);
            updateProgress(++count, total);
        }
    }

    /// <summary>Reads a DICOMDIR and enqueues all referenced files, reporting progress via callback.</summary>
    public async Task AddDicomDirAsync(string dicomDirPath, Action<int, int> updateProgress)
    {
        _dicomDirBasePath = Path.GetDirectoryName(dicomDirPath);
        Debug.WriteLine($"DICOMDIR base path: {_dicomDirBasePath}");

        DicomDirectory dicomDir = await DicomDirectory.OpenAsync(dicomDirPath);
        List<string> dicomFiles = [];
        TraverseDirectoryRecords(dicomDir.RootDirectoryRecord, dicomFiles);
        await AddDicomFilesAsync(dicomFiles, updateProgress);
    }

    /// <summary>Recursively collects all file paths referenced by DICOMDIR directory records.</summary>
    private void TraverseDirectoryRecords(DicomDirectoryRecord record, List<string> dicomFiles)
    {
        while (record != null)
        {
            if (record.Contains(DicomTag.ReferencedFileID))
            {
                string[] fileIDs = record.GetValues<string>(DicomTag.ReferencedFileID);
                string combinedPath = Path.GetFullPath(Path.Combine(_dicomDirBasePath!, Path.Combine(fileIDs)));
                if (File.Exists(combinedPath))
                    dicomFiles.Add(combinedPath);
                else
                    Debug.WriteLine($"File does not exist: {combinedPath}");
            }
            TraverseDirectoryRecords(record.LowerLevelDirectoryRecord, dicomFiles);
            record = record.NextDirectoryRecord;
        }
    }

    #endregion

    /// <summary>Dequeues and opens the next DICOM file. Returns null if the queue is empty.</summary>
    public async Task<DicomFile?> GetNextDicomFileAsync()
    {
        if (_dicomQueue.Count == 0) return null;
        return await DicomFile.OpenAsync(_dicomQueue.Dequeue());
    }

    /// <summary>Updates the PatientID tag for all files belonging to the given study.</summary>
    public async Task UpdatePatientIDInTempFolderAsync(string studyInstanceUID, string newPatientID, Action<int, int> updateProgress)
    {
        List<string> updatedFilePaths = [];
        EnsureModifiedFolderExists();

        string[] filePaths = Directory.GetFiles(_tempFolder);
        await ProcessFilesAsync(filePaths, studyInstanceUID, newPatientID, updateProgress, updatedFilePaths);
        UpdateDicomQueue(updatedFilePaths);
    }

    /// <summary>Clears the import queue.</summary>
    public void ResetQueue()
    {
        _dicomQueue.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    #region Private helpers

    /// <summary>Creates the modified folder on disk if it does not already exist.</summary>
    private void EnsureModifiedFolderExists()
    {
        if (!Directory.Exists(ModifiedFolder))
            Directory.CreateDirectory(ModifiedFolder);
    }

    /// <summary>
    /// Iterates over <paramref name="filePaths"/>, updating the PatientID for files
    /// that match <paramref name="studyInstanceUID"/>, and reports progress via <paramref name="updateProgress"/>.
    /// </summary>
    private async Task ProcessFilesAsync(string[] filePaths, string studyInstanceUID, string newPatientID,
        Action<int, int> updateProgress, List<string> updatedFilePaths)
    {
        int count = 0;
        foreach (string filePath in filePaths)
        {
            await ProcessSingleFileAsync(filePath, studyInstanceUID, newPatientID, updatedFilePaths);
            updateProgress(++count, filePaths.Length);
        }
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    /// <summary>
    /// Opens a single DICOM file and, if it belongs to the target study, updates its PatientID
    /// and saves it to the modified folder.
    /// </summary>
    private async Task ProcessSingleFileAsync(string filePath, string studyInstanceUID, string newPatientID,
        List<string> updatedFilePaths)
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
            dicomFile.Dataset.Clear();
            dicomFile = null!;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating patient ID in file {filePath}: {ex.Message}");
        }
    }

    /// <summary>Replaces the current queue contents with the supplied <paramref name="updatedFilePaths"/>.</summary>
    private void UpdateDicomQueue(List<string> updatedFilePaths)
    {
        _dicomQueue.Clear();
        foreach (string filePath in updatedFilePaths)
            _dicomQueue.Enqueue(filePath);
    }

    /// <summary>
    /// Copies a file to the temp folder using a unique name to avoid collisions,
    /// and returns the path of the copy.
    /// </summary>
    private async Task<string> CopyToTempFolderAsync(string filePath)
    {
        string tempFilePath = Path.Combine(_tempFolder,
            $"{Path.GetFileNameWithoutExtension(filePath)}_{Guid.NewGuid()}{Path.GetExtension(filePath)}");
        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Il file specificato non esiste.", filePath);

            await using FileStream src = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            await using FileStream dst = new(tempFilePath, FileMode.Create, FileAccess.Write);
            await src.CopyToAsync(dst);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new IOException($"Accesso negato durante la copia del file {filePath}.", ex);
        }
        catch (Exception ex) when (ex is not IOException)
        {
            throw new IOException($"Errore durante la copia del file {filePath}.", ex);
        }
        return tempFilePath;
    }

    #endregion
}
