namespace DicomModifier.Models
{
    public class PACSSettings
    {
        public string ServerIP { get; set; }
        public string ServerPort { get; set; }
        public string AETitle { get; set; }
        public string Timeout { get; set; }
        public string LocalAETitle { get; set; }

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
