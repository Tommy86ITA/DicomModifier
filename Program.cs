using System;
using System.Windows.Forms;

namespace DicomModifier
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm mainForm = new MainForm();
            TableManager tableManager = new TableManager(mainForm.DataGridView1);
            DicomManager dicomManager = new DicomManager(tableManager, mainForm);

            // Carica le impostazioni
            SettingsController settingsController = new SettingsController(mainForm);
            PACSSettings settings = settingsController.LoadSettings();

            MainController mainController = new MainController(mainForm, dicomManager, settings);

            Application.Run(mainForm);
        }
    }
}
