using FellowOakDicom;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;

namespace DicomModifier
{
    public class PACSCommunicator
    {
        private readonly PACSSettings _settings;
        private readonly ProgressManager _progressManager;

        public PACSCommunicator(PACSSettings settings, ProgressManager progressManager)
        {
            _settings = settings;
            _progressManager = progressManager;
        }

        public async Task<bool> SendCEcho()
        {
            try
            {
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
                var client = DicomClientFactory.Create(_settings.ServerIP, int.Parse(_settings.ServerPort), false, _settings.LocalAETitle, _settings.AETitle);

                foreach (var filePath in filePaths)
                {
                    var dicomFile = await DicomFile.OpenAsync(filePath).ConfigureAwait(false);
                    var cStoreRequest = new DicomCStoreRequest(dicomFile);

                    cStoreRequest.OnResponseReceived += (req, resp) =>
                    {
                        _progressManager.UpdateProgress(filePaths.IndexOf(filePath) + 1, filePaths.Count);
                    };

                    await client.AddRequestAsync(cStoreRequest).ConfigureAwait(false);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        _progressManager.UpdateStatus("Invio annullato dall'utente.");
                        return false;
                    }
                }

                await client.SendAsync(cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                _progressManager.UpdateStatus($"Errore durante l'invio dei file: {ex.Message}");
                return false;
            }
        }
    }
}
