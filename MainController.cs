using System;
using System.Windows.Forms;
using System.IO;
using DicomModifier;
using FellowOakDicom;

public class MainController
{
    private ConfigManager configManager;
    private DicomManager dicomManager;
    private DicomOperations dicomOperations;
    private MainForm mainForm;
    private int totalFiles;
    private int sentFiles;

    public MainController(MainForm form)
    {
        mainForm = form;
        configManager = new ConfigManager("config.json");
        dicomManager = new DicomManager();
        dicomOperations = new DicomOperations(configManager, dicomManager);

        mainForm.OnSelectFile += MainForm_OnSelectFile;
        mainForm.OnSelectFolder += MainForm_OnSelectFolder;
        mainForm.OnSelectDicomDir += MainForm_OnSelectDicomDir;
        mainForm.OnSend += MainForm_OnSend;
    }

    private void MainForm_OnSelectFile(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "DICOM Files|*.dcm";
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            dicomManager.AddDicomFile(openFileDialog.FileName);
            mainForm.AddDicomToGrid(DicomFile.Open(openFileDialog.FileName));
            totalFiles++;
            mainForm.UpdateFileCount(sentFiles, totalFiles);
        }
    }

    private void MainForm_OnSelectFolder(object sender, EventArgs e)
    {
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            var files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.dcm");
            foreach (var file in files)
            {
                dicomManager.AddDicomFile(file);
                mainForm.AddDicomToGrid(DicomFile.Open(file));
                totalFiles++;
            }
            mainForm.UpdateFileCount(sentFiles, totalFiles);
        }
    }

    private void MainForm_OnSelectDicomDir(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "DICOMDIR Files|DICOMDIR";
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            // Add logic to handle DICOMDIR files
        }
    }

    private void MainForm_OnSend(object sender, EventArgs e)
    {
        var dicomFile = dicomManager.GetNextDicomFile();
        if (dicomFile != null)
        {
            string newPatientId = mainForm.GetNewPatientId();
            if (string.IsNullOrEmpty(newPatientId))
            {
                if (MessageBox.Show("Vuoi inviare i file senza modifiche?", "Conferma invio", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    dicomOperations.SendDicomFile(dicomFile);
                    sentFiles++;
                }
            }
            else
            {
                if (MessageBox.Show($"Vuoi inviare i file con l'ID paziente modificato a {newPatientId}?", "Conferma invio", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    dicomOperations.ModifyDicomFile(dicomFile, newPatientId);
                    dicomOperations.SendDicomFile(dicomFile);
                    sentFiles++;
                }
            }
            mainForm.UpdateFileCount(sentFiles, totalFiles);
            mainForm.UpdateProgressBar((sentFiles * 100) / totalFiles);
        }
    }
}
