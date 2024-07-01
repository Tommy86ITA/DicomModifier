using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DicomModifier
{
    public class PACSSettings
    {
        public string ServerIP { get; set; }
        public string ServerPort { get; set; }
        public string AETitle { get; set; }
        public string Timeout { get; set; }
        public string LocalAETitle { get; set; }

        private static string configFilePath = "config.json";

        public static PACSSettings LoadSettings()
        {
            if (File.Exists(configFilePath))
            {
                var json = File.ReadAllText(configFilePath);
                return JsonSerializer.Deserialize<PACSSettings>(json);
            }

            return new PACSSettings(); // Return default settings if no config file is found
        }

        public void SaveSettings()
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, json);
        }
    }
}

