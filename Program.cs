using DicomModifier.Controllers;
using DicomModifier.Models;

namespace DicomModifier
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm mainForm = new();
            TableManager tableManager = new(mainForm.DataGridView1);
            DicomFileHandler dicomManager = new(tableManager, mainForm);

            // Carica le impostazioni
            SettingsController settingsController = new(mainForm);
            PACSSettings settings = settingsController.LoadSettings();

            MainController mainController = new(mainForm, dicomManager, settings);
            mainForm.Tag = mainController;

            Application.Run(mainForm);
        }
    }
}
