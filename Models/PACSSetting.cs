// Interfaces/PACSSetting.cs

namespace DicomModifier.Models
{
    /// <summary>
    /// Holds the PACS connection parameters and application-level preferences
    /// persisted in <c>Config.json</c>.
    /// </summary>
    public class PACSSettings
    {
        /// <summary>Gets or sets the PACS server IP address or hostname.</summary>
        public string ServerIP { get; set; } = "127.0.0.1";

        /// <summary>Gets or sets the PACS server port number.</summary>
        public string ServerPort { get; set; } = "104";

        /// <summary>Gets or sets the remote Application Entity title.</summary>
        public string AETitle { get; set; } = "PACS";

        /// <summary>Gets or sets the network timeout in milliseconds.</summary>
        public string Timeout { get; set; } = "30000";

        /// <summary>Gets or sets the local Application Entity title used during C-STORE/C-ECHO.</summary>
        public string LocalAETitle { get; set; } = "DICOM_MOD";

        /// <summary>
        /// Gets or sets a value indicating whether the optical drive should be ejected
        /// automatically after a successful import.
        /// </summary>
        public bool AutoEjectOpticalMedia { get; set; } = true;

        /// <summary>Resets all properties to their factory-default values.</summary>
        public void ApplyDefaults()
        {
            ServerIP = "127.0.0.1";
            ServerPort = "104";
            AETitle = "PACS";
            Timeout = "30000";
            LocalAETitle = "DICOM_MOD";
            AutoEjectOpticalMedia = true;
        }
    }
}
