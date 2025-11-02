using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class JsonDataService : IDataService
{
    public bool SaveData<T>(string ReletivePath, T Data, bool Encrypted)
    {
        string path = Application.persistentDataPath + ReletivePath;

        try
        {
            if (File.Exists(path))
            {
                Debug.Log("Data already exists. Deleting old file and writing a new one.");
                File.Delete(path);
            }
            else
            {
                Debug.Log("Writing file for the first time.");
            }
            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(Data));
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    public T LoadData<T>(string ReletivePath, bool Encrypted)
    {
        string path = Application.persistentDataPath + ReletivePath;

        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}. File does not exist.");
            throw new FileNotFoundException($"{path} does not exist.");
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        }
        catch(Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }
}
