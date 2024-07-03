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

            MainForm mainForm = new MainForm();
            TableManager tableManager = new TableManager(mainForm.DataGridView1);
            DicomFileHandler dicomManager = new DicomFileHandler(tableManager, mainForm);

            // Carica le impostazioni
            SettingsController settingsController = new SettingsController(mainForm);
            PACSSettings settings = settingsController.LoadSettings();

            MainController mainController = new MainController(mainForm, dicomManager, settings);
            mainForm.Tag = mainController;

            Application.Run(mainForm);
        }
    }
}
