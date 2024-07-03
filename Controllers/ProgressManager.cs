namespace DicomModifier.Controllers
{
    public class ProgressManager
    {
        private readonly MainForm _mainForm;

        public ProgressManager(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        public void UpdateStatus(string status)
        {
            _mainForm.UpdateStatus(status);
        }

        public void UpdateFileCount(int sent, int total)
        {
            _mainForm.UpdateFileCount(sent, total);
        }

        public void UpdateProgress(int sentFiles, int totalFiles)
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.Invoke(new Action(() =>
                {
                    _mainForm.UpdateFileCount(sentFiles, totalFiles);
                    _mainForm.UpdateProgressBar((int)((double)sentFiles / totalFiles * 100));
                    _mainForm.UpdateStatus($"Invio in corso... ({sentFiles}/{totalFiles} file inviati)");
                }));
            }
            else
            {
                _mainForm.UpdateFileCount(sentFiles, totalFiles);
                _mainForm.UpdateProgressBar((int)((double)sentFiles / totalFiles * 100));
                _mainForm.UpdateStatus($"Invio in corso... ({sentFiles}/{totalFiles} file inviati)");
            }
        }
    }
}
