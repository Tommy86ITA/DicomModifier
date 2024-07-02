using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using System.Windows.Forms;
using FellowOakDicom;

namespace DicomModifier
{
    public class PACSCommunicator
    {
        private readonly PACSSettings _settings;

        public PACSCommunicator(PACSSettings settings)
        {
            _settings = settings;
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

                client.AddRequestAsync(cEcho);
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

                foreach (var filePath in filePaths)
                {
                    var dicomFile = DicomFile.Open(filePath);
                    var cStore = new DicomCStoreRequest(dicomFile);

                    var tcs = new TaskCompletionSource<bool>();

                    cStore.OnResponseReceived += (req, resp) =>
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

                    await client.AddRequestAsync(cStore);
                }

                await client.SendAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'invio dei file: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
