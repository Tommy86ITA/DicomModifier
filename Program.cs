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
            DicomManager dicomManager = new DicomManager(tableManager, mainForm);

            MainController mainController = new MainController(mainForm, dicomManager);

            Application.Run(mainForm);
        }
    }
}
