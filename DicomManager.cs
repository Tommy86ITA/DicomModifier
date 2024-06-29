using FellowOakDicom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DicomModifier
{
    public class DicomManager
    {
        private TableManager _tableManager;

        public DicomManager(TableManager tableManager)
        {
            _tableManager = tableManager;
        }

        public void AddDicomFile(string filePath)
        {
            try
            {
                var dicomFile = DicomFile.Open(filePath);
                Debug.WriteLine($"Caricato file DICOM: {filePath}");
                _tableManager.AddDicomToGrid(dicomFile.Dataset);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore nel caricamento del file DICOM: {filePath}, Errore: {ex.Message}");
            }
        }

        public void AddDicomFolder(string folderPath)
        {
            var dicomFiles = Directory.GetFiles(folderPath, "*.dcm", SearchOption.AllDirectories);
            foreach (var dicomFile in dicomFiles)
            {
                AddDicomFile(dicomFile);
            }
        }
    }
}
