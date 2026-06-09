using DicomModifier.Models;
using FellowOakDicom;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using System.Diagnostics;

namespace DicomModifier.Services;

/// <summary>
/// Handles DICOM network communication (C-ECHO, C-STORE).
/// Has no dependency on UI components; callers are responsible for presenting feedback.
/// </summary>
public class PacsService(PACSSettings settings)
{
    private PACSSettings _settings = settings;

    /// <summary>Updates the PACS connection settings (e.g. after the user saves new settings).</summary>
    public void UpdateSettings(PACSSettings settings) => _settings = settings;

    /// <summary>
    /// Sends a C-ECHO request to verify the PACS connection.
    /// Throws on network/connection failure; returns false if the PACS replies with a non-success status.
    /// </summary>
    public async Task<bool> SendCEcho()
    {
        IDicomClient client = CreateClient();
        DicomCEchoRequest cEcho = new();
        TaskCompletionSource<bool> tcs = new();

        cEcho.OnResponseReceived += (_, resp) => tcs.SetResult(resp.Status == DicomStatus.Success);

        await client.AddRequestAsync(cEcho);
        await client.SendAsync();
        return await tcs.Task;
    }

    /// <summary>
    /// Sends a list of DICOM files via C-STORE, reporting progress through <paramref name="updateProgress"/>.
    /// Returns true on success, false on failure (exceptions are logged internally).
    /// </summary>
    public async Task<bool> SendFiles(List<string> filePaths, Action<int, int> updateProgress,
        CancellationToken cancellationToken = default)
    {
        try
        {
            IDicomClient client = CreateClient();

            foreach (string filePath in filePaths)
            {
                if (cancellationToken.IsCancellationRequested)
                    return false;

                DicomFile dicomFile = await DicomFile.OpenAsync(filePath).ConfigureAwait(false);
                DicomCStoreRequest cStoreRequest = new(dicomFile);
                cStoreRequest.OnResponseReceived += (_, _) =>
                    updateProgress(filePaths.IndexOf(filePath) + 1, filePaths.Count);

                await client.AddRequestAsync(cStoreRequest).ConfigureAwait(false);
            }

            await client.SendAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            Debug.Print($"Errore durante l'invio dei file: {ex.Message}");
            return false;
        }
    }

    /// <summary>Creates a configured <see cref="IDicomClient"/> from the current settings.</summary>
    private IDicomClient CreateClient() =>
        DicomClientFactory.Create(
            _settings.ServerIP,
            int.Parse(_settings.ServerPort),
            false,
            _settings.LocalAETitle,
            _settings.AETitle);
}
