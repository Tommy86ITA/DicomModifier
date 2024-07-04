// Interfaces/IProgressManager.cs
namespace DicomModifier.Interfaces
{
    public interface IProgressManager
    {
        /// <summary>
        /// Aggiorna il progresso dell'operazione di invio dei file.
        /// </summary>
        /// <param name="sentFiles">Il numero di file inviati finora.</param>
        /// <param name="totalFiles">Il numero totale di file da inviare.</param>
        void UpdateProgress(int sentFiles, int totalFiles);

        /// <summary>
        /// Aggiorna il messaggio di stato.
        /// </summary>
        /// <param name="status">Il messaggio di stato da visualizzare.</param>
        void UpdateStatus(string status);
    }
}
