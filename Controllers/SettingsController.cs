// Interfaces/SettingsController.cs

using DicomModifier;
using DicomModifier.Models;
using System.Text.Json;

namespace DicomModifier.Controllers
{
    /// <summary>
    /// The SettingsController class handles loading, saving, and managing PACS settings.
    /// </summary>
    public class SettingsController
    {
        /// <summary>
        /// The configuration file path in AppData.
        /// </summary>
        private static readonly string ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DicomModifier", "Config.json");

        private readonly MainForm _mainForm;
        private readonly PACSSettings _settings;

        /// <summary>
        /// JsonSerializerOptions to be used for all serialization and deserialization operations.
        /// </summary>
        private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsController"/> class.
        /// </summary>
        /// <param name="mainForm">The main form.</param>
        public SettingsController(MainForm mainForm)
        {
            _mainForm = mainForm;
            _settings = LoadSettings();
        }

        /// <summary>
        /// Checks if the Config.json file exists and if it's readable, then loads the settings.
        /// Otherwise, shows an error and calls the method that generates default settings.
        /// </summary>
        /// <returns>The loaded or default PACS settings.</returns>
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

        /// <summary>
        /// Saves the PACS settings to the configuration file.
        /// </summary>
        /// <param name="settings">The settings to save.</param>
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

        /// <summary>
        /// Creates the default PACS settings and saves them.
        /// </summary>
        /// <returns>The default PACS settings.</returns>
        private PACSSettings CreateDefaultSettings()
        {
            var defaultSettings = new PACSSettings();
            defaultSettings.ApplyDefaults();
            SaveSettings(defaultSettings);
            return defaultSettings;
        }
    }
}