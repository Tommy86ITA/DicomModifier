// Interfaces/IPACSCommunicator.cs
namespace DicomModifier.Interfaces
{
    public interface IPACSCommunicator
    {
        /// <summary>
        /// Invia una richiesta C-ECHO per verificare la connettività con il server PACS.
        /// </summary>
        /// <returns>Un task che rappresenta l'operazione asincrona. Il risultato del task contiene un booleano che indica il successo o il fallimento.</returns>
        Task<bool> SendCEcho();

        /// <summary>
        /// Invia una lista di file DICOM al server PACS.
        /// </summary>
        /// <param name="filePaths">La lista dei percorsi dei file da inviare.</param>
        /// <param name="cancellationToken">Un token per monitorare le richieste di annullamento.</param>
        /// <returns>Un task che rappresenta l'operazione asincrona. Il risultato del task contiene un booleano che indica il successo o il fallimento.</returns>
        Task<bool> SendFiles(List<string> filePaths, CancellationToken cancellationToken);
    }
}
