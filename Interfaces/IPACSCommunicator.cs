namespace DicomModifier.Interfaces
{
    public interface IPACSCommunicator
    {
        Task<bool> SendCEcho();
        Task<bool> SendFiles(CancellationToken cancellationToken);
        void EnqueueFile(string filePath);
        void ClearQueue();
    }
}
