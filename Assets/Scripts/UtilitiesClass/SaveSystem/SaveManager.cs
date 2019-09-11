using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// SaveManager
/// </summary>
public class SaveManager
{
    private static SaveManager instance = null;

    /// <summary>
    /// Get the singleton instance of the SaveManager
    /// </summary>
    public static SaveManager GetInstance()
    {
        if (instance == null)
            instance = new SaveManager();
        return instance;
    }

    public enum LoadState { SUBSTITUTED, CREATED, LOADED }

    public struct SaveResult
    {
        public SaveObject saveObject;
        public LoadState loadState;
    }

    //Can write here the statics file names to use in the game
    //
    public static readonly string PLAYER_DATA = Application.persistentDataPath + "/player_data.data";
    public static readonly string GRAVITYPOINTS_PATH = Application.persistentDataPath + "/gravity_points.data";
    public static readonly string SKILLSDATA_PATH = Application.persistentDataPath + "/skills.data";
    public static readonly string SETTINGS_PATH = Application.persistentDataPath + "/settings.data";
    public static readonly string ACHIEVMENTS_PATH = Application.persistentDataPath + "/achievments_v2.data";
    public static readonly string LEVELSDATA_PATH = Application.persistentDataPath + "/levels.data";
    //I.E 
    //public static readonly string PLAYER_DATA = Application.persistentDataPath + "/player_data.data";
    //Calling the methods will look like:
    //SaveManager.GetInstance().SavePersistentData<T>(data, SaveManager.PLAYER_DATA);

    /// <summary>
    /// Save a generic type of data in the application persisten data path.
    /// <para>Returns a SaveObject that can be used to check if data has been saved correctly and to retrieve data</para>
    /// </summary>
    public SaveObject SavePersistentData<T>(T data, string path)
    {
        SaveObject saveObject = new SaveObject(data);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
        return saveObject;
    }

    /// <summary>
    /// Load a SaveObject that contains the type of data in the selected path.
    /// <para>Return a SaveObject. Use saveObject.GetData() to retrieve data. If the data is not present a null SaveObject will be returned</para>
    /// </summary>
    public SaveObject LoadPersistentData(string path)
    {
        SaveObject saveObject;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length == 0)
                return null;
            object data = formatter.Deserialize(stream);
            if(data is EncryptedData)
            {
                EncryptedData enc = (EncryptedData)data;
                if(enc.deviceId != SystemInfo.deviceUniqueIdentifier)
                {
                    Debug.Log("Unauthorized to open file, file not coming from this device, aborting");
                    stream.Close();
                    return null;
                }
            }
            saveObject = new SaveObject(data);
            stream.Close();
            return saveObject;
        }
        else
        {
            return null;
        }
    }
}
