// Interfaces/Program.cs

using DicomModifier.Controllers;
using DicomModifier.Models;
using System.Reflection;
using System.Runtime.InteropServices; // Aggiungi questo using

namespace DicomModifier
{
    internal static class Program
    {
        private static string? appGuid;

        static Program()
        {
            // Ottieni l'attributo GUID dell'assembly
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true);
            if (attributes.Length > 0 && attributes[0] is GuidAttribute guidAttribute)
            {
                appGuid = guidAttribute.Value;
            }
            else
            {
                // Se non c'è un attributo GUID, utilizza un GUID di default
                appGuid = "18116A9D-E35C-4AB5-8D15-0F15FD1B5DFE";
            }
        }

        [STAThread]
        private static void Main()
        {
            using (Mutex mutex = new Mutex(false, "Global\\" + appGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("Instance already running");
                    return;
                }

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
}