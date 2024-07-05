using DicomModifier.Interfaces;
using DicomModifier.Models;
using FellowOakDicom;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DicomModifier.Controllers
{
    public class PACSCommunicator : IPACSCommunicator
    {
        private readonly PACSSettings _settings;
        private readonly IProgressManager _progressManager;
        private readonly Queue<string> dicomQueue;

        // Costruttore: inizializza le impostazioni PACS e il gestore del progresso
        public PACSCommunicator(PACSSettings settings, IProgressManager progressManager)
        {
            _settings = settings;
            _progressManager = progressManager;
            dicomQueue = new Queue<string>();
        }

        // Metodo per inviare un C-ECHO per verificare la connessione al server PACS
        public async Task<bool> SendCEcho()
        {
            try
            {
                // Crea un client DICOM per il C-ECHO
                var client = DicomClientFactory.Create(_settings.ServerIP, int.Parse(_settings.ServerPort), false, _settings.LocalAETitle, _settings.AETitle);
                var cEcho = new DicomCEchoRequest();

                var tcs = new TaskCompletionSource<bool>();

                cEcho.OnResponseReceived += (req, resp) =>
                {
                    if (resp.Status == DicomStatus.Success)
                    {
                        tcs.SetResult(true);
                    }
                    else
                    {
                        tcs.SetResult(false);
                    }
                };

                // Aggiungi e invia la richiesta C-ECHO
                await client.AddRequestAsync(cEcho);
                await client.SendAsync();
                return await tcs.Task;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il C-ECHO: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Metodo per aggiungere un file alla coda
        public void EnqueueFile(string filePath)
        {
            dicomQueue.Enqueue(filePath);
        }

        // Metodo per svuotare la coda
        public void ClearQueue()
        {
            dicomQueue.Clear();
        }

        // Metodo per inviare una lista di file DICOM al server PACS
        public async Task<bool> SendFiles(List<string> filePaths, CancellationToken cancellationToken)
        {
            try
            {
                // Crea un client DICOM per l'invio dei file
                var client = DicomClientFactory.Create(_settings.ServerIP, int.Parse(_settings.ServerPort), false, _settings.LocalAETitle, _settings.AETitle);

                _progressManager.UpdateProgress(0, filePaths.Count);

                foreach (var filePath in filePaths)
                {
                    // Apre il file DICOM e crea una richiesta C-STORE per ciascun file
                    var dicomFile = await DicomFile.OpenAsync(filePath).ConfigureAwait(false);
                    var cStoreRequest = new DicomCStoreRequest(dicomFile);

                    cStoreRequest.OnResponseReceived += (req, resp) =>
                    {
                        _progressManager.UpdateProgress(filePaths.IndexOf(filePath) + 1, filePaths.Count);
                    };

                    // Aggiungi la richiesta al client
                    await client.AddRequestAsync(cStoreRequest).ConfigureAwait(false);

                    // Controlla se la cancellazione è stata richiesta
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _progressManager.UpdateStatus("Invio annullato dall'utente.");
                        return false;
                    }
                }

                // Invia tutte le richieste al server PACS
                await client.SendAsync(cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                _progressManager.UpdateStatus($"Errore durante l'invio dei file: {ex.Message}");
                return false;
            }
        }

        // Metodo per inviare i file nella coda al server PACS
        public async Task<bool> SendFiles(CancellationToken cancellationToken)
        {
            return await SendFiles(new List<string>(dicomQueue), cancellationToken);
        }
    }
}
