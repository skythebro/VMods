using System;
using System.IO;
using System.Text.Json;
using VMods.Shared;

namespace VMods.BloodRefill.Commands;

public class SaveSystem
{
    #region Consts
		
    private static string ConfigStoragePath =  $"{BepInEx.Paths.ConfigPath}\\VMods\\Storage";
		
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        IncludeFields = false,
    };

    #endregion

    #region Events

    public delegate void SaveEventHandler();
        
    public static event SaveEventHandler SaveEvent;
    private static void FireSaveEvent() => SaveEvent?.Invoke();
        
        
    #endregion
    public static void SaveAll() => FireSaveEvent();
    
    
    
    public static void Save<T>(string filename, T data)
    {
        try
        {
            File.WriteAllText(Path.Combine(ConfigStoragePath, filename), JsonSerializer.Serialize(data, JsonOptions));
#if DEBUG
            Utils.Logger.LogInfo($"{filename} has been saved.");
#endif
        }
        catch(Exception ex)
        {
            Utils.Logger.LogError($"Failed to save {filename}! - Error: {ex.Message}\r\n{ex.StackTrace}");
        }
    }

    public static T Load<T>(string filename, Func<T> getDefaultValue)
    {
        try
        {
            if(!Directory.Exists(ConfigStoragePath))
            {
                Directory.CreateDirectory(ConfigStoragePath);
            }
            var fullPath = Path.Combine(ConfigStoragePath, filename);
#if DEBUG
            Utils.Logger.LogMessage($"Loading {fullPath}");
#endif			
				
            if(!File.Exists(fullPath))
            {
                return getDefaultValue();
            }
            string json = File.ReadAllText(fullPath);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch(Exception ex)
        {
            Utils.Logger.LogError($"Failed to load {filename}! - Error: {ex.Message}\r\n{ex.StackTrace}");
            return getDefaultValue();
        }
    }
}