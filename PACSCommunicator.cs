using FellowOakDicom.Network;
using FellowOakDicom;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using FellowOakDicom.Network.Client;

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
    }
}
