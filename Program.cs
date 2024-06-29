using System;
using System.Windows.Forms;

namespace DicomModifier
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm mainForm = new MainForm();
            DicomManager dicomManager = new DicomManager(mainForm.TableManager);
            MainController mainController = new MainController(mainForm, dicomManager);

            Application.Run(mainForm);
        }
    }
}
