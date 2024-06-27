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
            MainController mainController = new MainController(mainForm);
            Application.Run(mainForm);
        }
    }
}
