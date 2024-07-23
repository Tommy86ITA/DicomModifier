// Interfaces/Program.cs

using DicomModifier.Controllers;
using DicomModifier.Models;
using DicomModifier.Services; // Aggiungi questa direttiva using
using DicomModifier.Views; // Aggiungi questa direttiva using
using System.Reflection;
using System.Runtime.InteropServices;

namespace DicomModifier
{
    internal static class Program
    {
        private static readonly string? appGuid;

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
            using Mutex mutex = new(false, "Global\\" + appGuid);
            if (!mutex.WaitOne(0, false))
            {
                MessageBox.Show("Dicom Import & Edit è già in esecuzione!");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Creiamo il servizio di autenticazione
            AuthenticationService authService = new AuthenticationService();

            // Mostriamo il form di login
            using (LoginForm loginForm = new LoginForm(authService))
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return; // Chiude l'applicazione se il login fallisce
                }
            }

            MainForm mainForm = new(authService);
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