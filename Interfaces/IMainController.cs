namespace DicomModifier.Interfaces
{
    public interface IMainController
    {
        void CancelSending();
        void MainForm_OnSelectFile(object sender, EventArgs e);
        void MainForm_OnSelectFolder(object sender, EventArgs e);
        void MainForm_OnSelectDicomDir(object sender, EventArgs e);
        void MainForm_OnSend(object sender, EventArgs e);
        void MainForm_OnResetQueue(object sender, EventArgs e);
        void MainForm_OnUpdatePatientID(object sender, EventArgs e);
    }
}
