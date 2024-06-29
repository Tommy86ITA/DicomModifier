namespace DicomModifier
{
    public class MainController
    {
        private MainForm _mainForm;
        private DicomManager _dicomManager;

        public MainController(MainForm mainForm, DicomManager dicomManager)
        {
            _mainForm = mainForm;
            _dicomManager = dicomManager;

            _mainForm.OnSelectFile += MainForm_OnSelectFile;
            _mainForm.OnSelectFolder += MainForm_OnSelectFolder;
            _mainForm.OnSelectDicomDir += MainForm_OnSelectDicomDir;
            _mainForm.OnSend += MainForm_OnSend;
        }

        private void MainForm_OnSelectFile(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _dicomManager.AddDicomFile(openFileDialog.FileName);
                    _mainForm.TableManager.AddDicomToGrid(_dicomManager.GetNextDicomFile().Dataset);
                }
            }
        }

        private void MainForm_OnSelectFolder(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    _dicomManager.AddDicomFolder(folderBrowserDialog.SelectedPath);
                    while (_dicomManager.DicomQueueCount > 0)
                    {
                        _mainForm.TableManager.AddDicomToGrid(_dicomManager.GetNextDicomFile().Dataset);
                    }
                }
            }
        }

        private void MainForm_OnSelectDicomDir(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _dicomManager.AddDicomDir(openFileDialog.FileName);
                    while (_dicomManager.DicomQueueCount > 0)
                    {
                        _mainForm.TableManager.AddDicomToGrid(_dicomManager.GetNextDicomFile().Dataset);
                    }
                }
            }
        }

        private void MainForm_OnSend(object sender, EventArgs e)
        {
            // Implement the logic for sending DICOM files
        }
    }
}
