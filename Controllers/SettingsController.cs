// Interfaces/SettingsController.cs

using DicomModifier.Models;
using System.Text.Json;

namespace DicomModifier.Controllers
{
    public class SettingsController
    {
        /// <summary>
        /// The configuration file path
        /// </summary>
        private const string ConfigFilePath = "Config.json";

        private readonly MainForm _mainForm;
        private readonly PACSSettings _settings;

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
        /// Checks if the Config.json file exists and if itìs readable, then loads the settings. Otherwise shows an error and calls the method that generates default settings.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Text.Json.JsonException">Deserializzazione fallita.</exception>
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
                PACSSettings? settings = JsonSerializer.Deserialize<PACSSettings>(json) ?? throw new JsonException("Deserializzazione fallita.");
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
        /// Saves the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void SaveSettings(PACSSettings settings)
        {
            try
            {
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFilePath, json);
                _mainForm.UpdateStatus("Impostazioni salvate correttamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il salvataggio delle impostazioni: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creates the default settings.
        /// </summary>
        /// <returns></returns>
        private PACSSettings CreateDefaultSettings()
        {
            var defaultSettings = new PACSSettings();
            defaultSettings.ApplyDefaults();
            SaveSettings(defaultSettings);
            return defaultSettings;
        }
    }
}