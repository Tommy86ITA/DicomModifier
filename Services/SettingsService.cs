using DicomModifier.Models;
using System.Text.Json;

namespace DicomModifier.Services;

/// <summary>
/// Provides static methods for loading and saving PACS settings.
/// Has no dependency on UI components; callers are responsible for presenting feedback.
/// </summary>
public static class SettingsService
{
    private static readonly string ConfigFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DicomModifier",
        "Config.json");

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Loads settings from disk. Returns defaults if the file is missing, empty, or corrupt.
    /// </summary>
    public static PACSSettings LoadSettings()
    {
        if (!File.Exists(ConfigFilePath) || new FileInfo(ConfigFilePath).Length == 0)
            return CreateAndSaveDefaults();

        try
        {
            string json = File.ReadAllText(ConfigFilePath);
            return JsonSerializer.Deserialize<PACSSettings>(json, JsonOptions)
                ?? throw new JsonException("Deserializzazione fallita.");
        }
        catch
        {
            return CreateAndSaveDefaults();
        }
    }

    /// <summary>Saves settings to disk. Throws <see cref="IOException"/> on failure.</summary>
    public static void SaveSettings(PACSSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        string? dir = Path.GetDirectoryName(ConfigFilePath);
        if (dir != null && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string json = JsonSerializer.Serialize(settings, JsonOptions);
        File.WriteAllText(ConfigFilePath, json);
    }

    /// <summary>Creates a default <see cref="PACSSettings"/> instance, persists it to disk, and returns it.</summary>
    private static PACSSettings CreateAndSaveDefaults()
    {
        PACSSettings defaults = new();
        defaults.ApplyDefaults();
        try { SaveSettings(defaults); } catch { /* non fatale */ }
        return defaults;
    }
}
