using System.Text.Json;

namespace DicomModifier
{
    public class SettingsController
    {
        private const string ConfigFilePath = "Config.json";
        private readonly MainForm _mainForm;


        public MainForm GetMainForm()
        {
            return _mainForm;
        }


        public SettingsController(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        public PACSSettings LoadSettings()
        {
            if (!File.Exists(ConfigFilePath))
            {
                MessageBox.Show("Il file di configurazione non esiste. Verranno utilizzate le impostazioni predefinite.", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return new PACSSettings();
            }

            try
            {
                var json = File.ReadAllText(ConfigFilePath);
                var settings = JsonSerializer.Deserialize<PACSSettings>(json);
                _mainForm.UpdateStatus("Impostazioni caricate correttamente.");
                return settings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il caricamento delle impostazioni: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new PACSSettings();
            }
        }

        public void SaveSettings(PACSSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFilePath, json);
                _mainForm.UpdateStatus("Impostazioni salvate correttamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il salvataggio delle impostazioni: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
