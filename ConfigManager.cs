using Newtonsoft.Json;

public class ConfigManager
{
    private readonly string configFilePath;
    private dynamic configData;

    public ConfigManager(string filePath)
    {
        configFilePath = filePath;
        LoadConfig();
    }

    private void LoadConfig()
    {
        if (File.Exists(configFilePath))
        {
            var json = File.ReadAllText(configFilePath);
            configData = JsonConvert.DeserializeObject(json);
        }
        else
        {
            configData = new { }; // Load default configuration
        }
    }

    public string GetConfigValue(string key)
    {
        return configData[key];
    }

    public void SetConfigValue(string key, string value)
    {
        configData[key] = value;
        SaveConfig();
    }

    private void SaveConfig()
    {
        var json = JsonConvert.SerializeObject(configData, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(configFilePath, json);
    }
}

