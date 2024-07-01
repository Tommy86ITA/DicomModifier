using System;
using System.IO;
using Newtonsoft.Json;

namespace DicomModifier
{
    public class SettingsController
    {
        private const string ConfigFilePath = "Config.json";
        private MainForm _mainForm;

        public SettingsController(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        public PACSSettings LoadSettings()
        {
            if (!File.Exists(ConfigFilePath))
            {
                _mainForm.UpdateStatus("Config.json non trovato. Impostazioni non caricate.");
                return new PACSSettings();
            }

            try
            {
                var json = File.ReadAllText(ConfigFilePath);
                var settings = JsonConvert.DeserializeObject<PACSSettings>(json);
                _mainForm.UpdateStatus("Impostazioni caricate correttamente.");
                return settings;
            }
            catch (Exception ex)
            {
                _mainForm.UpdateStatus($"Errore durante il caricamento delle impostazioni: {ex.Message}");
                return new PACSSettings();
            }
        }

        public void SaveSettings(PACSSettings settings)
        {
            try
            {
                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json);
                _mainForm.UpdateStatus("Impostazioni salvate correttamente.");
            }
            catch (Exception ex)
            {
                _mainForm.UpdateStatus($"Errore durante il salvataggio delle impostazioni: {ex.Message}");
            }
        }
    }
}
