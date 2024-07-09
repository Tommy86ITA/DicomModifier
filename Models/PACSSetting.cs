using System.Text.Json;

namespace DicomModifier.Models
{
    public class PACSSettings
    {
        public string ServerIP { get; set; }
        public string ServerPort { get; set; }
        public string AETitle { get; set; }
        public string Timeout { get; set; }
        public string LocalAETitle { get; set; }

        private static readonly string configFilePath = "config.json";

        public static PACSSettings LoadSettings()
        {
            if (File.Exists(configFilePath))
            {
                string json = File.ReadAllText(configFilePath);
                return JsonSerializer.Deserialize<PACSSettings>(json);
            }

            return new PACSSettings(); // Return default settings if no config file is found
        }

        public void SaveSettings()
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, json);
        }
    }
}

