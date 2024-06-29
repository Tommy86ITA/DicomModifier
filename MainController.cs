using System;
using System.Windows.Forms;

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
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "DICOM Files (*.dcm)|*.dcm";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _dicomManager.AddDicomFile(openFileDialog.FileName);
                }
            }
        }

        private void MainForm_OnSelectFolder(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    _dicomManager.AddDicomFolder(folderBrowserDialog.SelectedPath);
                }
            }
        }

        private void MainForm_OnSelectDicomDir(object sender, EventArgs e)
        {
            //using (OpenFileDialog openFileDialog = new OpenFileDialog())
            //{
            //    openFileDialog.Filter = "DICOMDIR Files (DICOMDIR)|DICOMDIR";
            //    if (openFileDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        _dicomManager.AddDicomDir(openFileDialog.FileName);
            //    }
            //}
        }

        private void MainForm_OnSend(object sender, EventArgs e)
        {
            // Implement sending logic here
        }
    }
}
