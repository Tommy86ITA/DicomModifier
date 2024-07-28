// Import necessary namespaces for controllers, models, services, and views of the application.
using DicomModifier.Controllers;
using DicomModifier.Models;
using DicomModifier.Services;
using DicomModifier.Views;
using System.Reflection; // For working with assembly metadata.
using System.Runtime.InteropServices; // For interoperability services.

namespace DicomModifier
{
    internal static class Program
    {
        // Field to store the application GUID.
        private static readonly string? appGuid;

        static Program()
        {
            // Get the GUID attribute from the assembly.
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true);
            if (attributes.Length > 0 && attributes[0] is GuidAttribute guidAttribute)
            {
                appGuid = guidAttribute.Value; // Assign the assembly GUID value to appGuid.
            }
            else
            {
                // Use a default GUID if no GUID attribute is found.
                appGuid = "18116A9D-E35C-4AB5-8D15-0F15FD1B5DFE";
            }
        }

        [STAThread]
        private static void Main()
        {
            // Create a mutex to ensure only one instance of the application is running.
            using Mutex mutex = new(false, "Global\\" + appGuid);
            if (!mutex.WaitOne(0, false))
            {
                // Show a message if the application is already running.
                MessageBox.Show("Dicom Import & Edit is already running!");
                return;
            }

            // Enable visual styles and set compatible text rendering.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create the authentication service.
            AuthenticationService authService = new();

            // Show the login form.
            using (LoginForm loginForm = new(authService))
            {
                // Exit the application if login fails.
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }

            // Create the main form and controllers.
            MainForm mainForm = new(authService);
            UIController uiController = new(mainForm);
            DicomFileHandler dicomManager = new(uiController);

            // Load PACS settings.
            SettingsController settingsController = new(mainForm);
            PACSSettings settings = settingsController.LoadSettings();

            // Create the main controller and assign the main form.
            MainController mainController = new(mainForm, dicomManager, settings);
            mainForm.Tag = mainController;

            // Run the application with the main form.
            Application.Run(mainForm);
        }
    }
}
