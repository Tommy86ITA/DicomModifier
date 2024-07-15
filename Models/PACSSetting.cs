// Interfaces/PACSSetting.cs

namespace DicomImport.Models
{
    public class PACSSettings
    {
        public string ServerIP { get; set; } = "127.0.0.1";
        public string ServerPort { get; set; } = "104";
        public string AETitle { get; set; } = "PACS";
        public string Timeout { get; set; } = "30000";
        public string LocalAETitle { get; set; } = "DICOM_MOD";

        public void ApplyDefaults()
        {
            ServerIP = "127.0.0.1";
            ServerPort = "104";
            AETitle = "PACS";
            Timeout = "30000";
            LocalAETitle = "DICOM_MOD";
        }
    }
}