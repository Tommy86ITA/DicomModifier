// Interfaces/PACSCommunicator.cs

using DicomModifier.Models;
using FellowOakDicom;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using System.Diagnostics;

namespace DicomModifier.Controllers
{
    public class PACSCommunicator(PACSSettings settings, UIController uiController)
    {
        private readonly PACSSettings _settings = settings;
        private readonly UIController _uiController = uiController;

        public async Task<bool> SendCEcho()
        {
            try
            {
                IDicomClient client = DicomClientFactory.Create(_settings.ServerIP, int.Parse(_settings.ServerPort), false, _settings.LocalAETitle, _settings.AETitle);
                DicomCEchoRequest cEcho = new();

                TaskCompletionSource<bool> tcs = new();

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

        public async Task<bool> SendFiles(List<string> filePaths, CancellationToken cancellationToken)
        {
            try
            {
                IDicomClient client = DicomClientFactory.Create(_settings.ServerIP, int.Parse(_settings.ServerPort), false, _settings.LocalAETitle, _settings.AETitle);

                if (filePaths.Count == 0)
                {
                    _uiController.UpdateStatus("Nessun file da inviare.");
                    return false;
                }

                _uiController.UpdateStatus("Inizio invio file...");
                _uiController.UpdateProgress(0, filePaths.Count);

                foreach (string filePath in filePaths)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _uiController.UpdateStatus("Invio annullato dall'utente.");
                        return false;
                    }

                    DicomFile dicomFile = await DicomFile.OpenAsync(filePath).ConfigureAwait(false);
                    DicomCStoreRequest cStoreRequest = new(dicomFile);

                    cStoreRequest.OnResponseReceived += (req, resp) => _uiController.UpdateProgress(filePaths.IndexOf(filePath) + 1, filePaths.Count);

                    await client.AddRequestAsync(cStoreRequest).ConfigureAwait(false);
                }

                await client.SendAsync(cancellationToken).ConfigureAwait(false);

                _uiController.UpdateStatus("Invio completato.");
                return true;
            }
            catch (Exception ex)
            {
                _uiController.UpdateStatus($"Errore durante l'invio dei file: {ex.Message}");
                Debug.Print($"Errore durante l'invio dei file: {ex.Message}");
                return false;
            }
        }
    }
}