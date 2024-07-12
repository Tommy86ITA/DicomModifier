// Interfaces/Program.cs

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
            UIController uiController = new(mainForm);
            DicomFileHandler dicomManager = new(uiController);

            SettingsController settingsController = new(mainForm);
            PACSSettings settings = settingsController.LoadSettings();

            MainController mainController = new(mainForm, dicomManager, settings);
            mainForm.Tag = mainController;

            Application.Run(mainForm);
        }
    }
}
