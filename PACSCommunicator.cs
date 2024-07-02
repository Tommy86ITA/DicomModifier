using FellowOakDicom;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public async Task<bool> SendFiles(List<string> filePaths)
        {
            try
            {
                var client = DicomClientFactory.Create(_settings.ServerIP, int.Parse(_settings.ServerPort), false, _settings.LocalAETitle, _settings.AETitle);
                var tcs = new TaskCompletionSource<bool>();

                int totalFiles = filePaths.Count;
                int sentFiles = 0;

                foreach (var filePath in filePaths)
                {
                    var dicomFile = await DicomFile.OpenAsync(filePath);
                    var cStoreRequest = new DicomCStoreRequest(dicomFile);

                    cStoreRequest.OnResponseReceived += (req, resp) =>
                    {
                        if (resp.Status == DicomStatus.Success)
                        {
                            sentFiles++;
                            _progressManager.UpdateProgress(sentFiles, totalFiles);
                        }
                        else
                        {
                            tcs.SetException(new Exception($"Failed to send DICOM message C-STORE request [2] for file: {filePath}"));
                        }
                    };

                    await client.AddRequestAsync(cStoreRequest);
                }

                await client.SendAsync();
                tcs.SetResult(sentFiles == totalFiles);
                return await tcs.Task;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'invio dei file: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
