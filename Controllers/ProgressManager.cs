using DicomModifier.Interfaces;

namespace DicomModifier.Controllers
{
    public class ProgressManager : IProgressManager
    {
        private readonly MainForm _mainForm;

        public ProgressManager(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        // Metodo per aggiornare lo stato nella barra di stato
        public void UpdateStatus(string status)
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.Invoke(new Action(() => _mainForm.UpdateStatus(status)));
            }
            else
            {
                _mainForm.UpdateStatus(status);
            }
        }

        // Metodo per aggiornare il contatore dei file inviati
        public void UpdateFileCount(int sent, int total)
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.Invoke(new Action(() => _mainForm.UpdateFileCount(sent, total)));
            }
            else
            {
                _mainForm.UpdateFileCount(sent, total);
            }
        }

        // Metodo per aggiornare la barra di progresso e lo stato
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
