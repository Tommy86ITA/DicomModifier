//using DicomModifier.Models;
//using Newtonsoft.Json;

//public class ConfigManager
//{
//    private readonly string configFilePath;
//    private dynamic configData;

//    public ConfigManager(string filePath)
//    {
//        configFilePath = filePath;
//        LoadConfig();
//    }

//    private void LoadConfig()
//    {
//        if (File.Exists(configFilePath))
//        {
//            string json = File.ReadAllText(configFilePath);
//#pragma warning disable CS8601 // Possibile assegnazione di riferimento Null. // Non può mai essere Null, perchè viene verificata l'esistenza del file
//            configData = JsonConvert.DeserializeObject(json);
//#pragma warning restore CS8601 // Possibile assegnazione di riferimento Null.
//        }
//        else
//        {
//            configData = new 
//            {

//            }; // Load default configuration
//        }
//    }

//    public string GetConfigValue(string key)
//    {
//        return configData[key];
//    }

//    public void SetConfigValue(string key, string value)
//    {
//        configData[key] = value;
//        SaveConfig();
//    }

//    private void SaveConfig()
//    {
//        dynamic json = JsonConvert.SerializeObject(configData, Newtonsoft.Json.Formatting.Indented);
//        File.WriteAllText(configFilePath, json);
//    }
//}

using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

public class ConfigManager
{
    private readonly string configFilePath;
    private dynamic configData;

    public ConfigManager(string filePath)
    {
        configFilePath = filePath;
        LoadConfig();
        if (configData == null)
        {
            SetDefaultConfig();
            SaveConfig();
        }
    }

    private void LoadConfig()
    {
        if (File.Exists(configFilePath))
        {
            string json = File.ReadAllText(configFilePath);
#pragma warning disable CS8601 // Possibile assegnazione di riferimento Null.
            configData = JsonConvert.DeserializeObject(json);
#pragma warning restore CS8601 // Possibile assegnazione di riferimento Null.
            if (configData == null)
            {
                MessageBox.Show("Errore durante il caricamento della configurazione. Verranno utilizzate le impostazioni predefinite.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetDefaultConfig();
                
            }
        }
        else
        {
            SetDefaultConfig();
        }
    }

    private void SetDefaultConfig()
    {
        configData = new
        {
            AETitle = "PACS",
            LocalAETitle = "DICOM_MOD",
            ServerIP = "127.0.0.1",
            ServerPort = "104",
            Timeout = "30000"
        };
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
        var json = JsonConvert.SerializeObject(configData, Formatting.Indented);
        File.WriteAllText(configFilePath, json);
    }
}

