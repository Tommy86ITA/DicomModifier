// Interfaces/SettingsController.cs

using DicomModifier.Models;
using System.Text.Json;

namespace DicomModifier.Controllers
{
    public class SettingsController
    {
        private static readonly string ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DicomModifier", "Config.json");

        private readonly MainForm _mainForm;
        private readonly PACSSettings _settings;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

        public SettingsController(MainForm mainForm)
        {
            _mainForm = mainForm;
            _settings = LoadSettings();
        }

        public PACSSettings LoadSettings()
        {
            if (!File.Exists(ConfigFilePath) || new FileInfo(ConfigFilePath).Length == 0)
            {
                MessageBox.Show("Il file di configurazione non esiste o è vuoto. Verranno utilizzate le impostazioni predefinite.", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return CreateDefaultSettings();
            }

            try
            {
                string json = File.ReadAllText(ConfigFilePath);
                PACSSettings settings = JsonSerializer.Deserialize<PACSSettings>(json, jsonSerializerOptions) ?? throw new JsonException("Deserializzazione fallita.");
                _mainForm.UpdateStatus("Impostazioni caricate correttamente.");
                return settings;
            }
            catch (Exception)
            {
                MessageBox.Show("Errore durante il caricamento delle impostazioni. Verranno utilizzate le impostazioni predefinite.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return CreateDefaultSettings();
            }
        }

        public void SaveSettings(PACSSettings settings)
        {
            try
            {
                string? directoryPath = Path.GetDirectoryName(ConfigFilePath);
                if (directoryPath != null && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string json = JsonSerializer.Serialize(settings, jsonSerializerOptions);
                File.WriteAllText(ConfigFilePath, json);
                _mainForm.UpdateStatus("Impostazioni salvate correttamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il salvataggio delle impostazioni: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private PACSSettings CreateDefaultSettings()
        {
            var defaultSettings = new PACSSettings();
            defaultSettings.ApplyDefaults();
            SaveSettings(defaultSettings);
            return defaultSettings;
        }
    }
}