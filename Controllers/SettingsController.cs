//using DicomModifier.Models;
//using System.Text.Json;

//namespace DicomModifier.Controllers
//{
//    public class SettingsController
//    {
//        private const string ConfigFilePath = "Config.json";
//        private readonly MainForm _mainForm;

//        public MainForm GetMainForm()
//        {
//            return _mainForm;
//        }

//        public SettingsController(MainForm mainForm)
//        {
//            _mainForm = mainForm;
//        }

//        public PACSSettings LoadSettings()
//        {
//            if (!File.Exists(ConfigFilePath))
//            {
//                MessageBox.Show("Il file di configurazione non esiste. Verranno utilizzate le impostazioni predefinite.", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                new PACSSettings();

//            }

//            try
//            {
//                string json = File.ReadAllText(ConfigFilePath);
//                PACSSettings? settings = JsonSerializer.Deserialize<PACSSettings>(json);
//                _mainForm.UpdateStatus("Impostazioni caricate correttamente.");
//                return settings;
//            }
//            catch (Exception)
//            {
//                MessageBox.Show($"Errore durante il caricamento delle impostazioni. Verrano utilizzate le impostazioni predefinite.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                return new PACSSettings();
//            }
//        }

//        public void SaveSettings(PACSSettings settings)
//        {
//            try
//            {
//                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
//                File.WriteAllText(ConfigFilePath, json);
//                _mainForm.UpdateStatus("Impostazioni salvate correttamente.");
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Errore durante il salvataggio delle impostazioni: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }
//    }
//}

using DicomModifier.Models;
using System.Text.Json;

namespace DicomModifier.Controllers
{
    public class SettingsController
    {
        private const string ConfigFilePath = "Config.json";
        private readonly MainForm _mainForm;
        private PACSSettings _settings;

        public MainForm GetMainForm()
        {
            return _mainForm;
        }

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

        private PACSSettings CreateDefaultSettings()
        {
            var defaultSettings = new PACSSettings();
            defaultSettings.ApplyDefaults();
            SaveSettings(defaultSettings);
            return defaultSettings;
        }

        public PACSSettings GetSettings()
        {
            return _settings;
        }

        public void UpdateSettings(PACSSettings settings)
        {
            _settings = settings;
            SaveSettings(settings);
        }
    }
}
