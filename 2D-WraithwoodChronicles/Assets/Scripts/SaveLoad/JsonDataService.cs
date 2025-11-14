using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class JsonDataService : IDataService
{
    private static JsonSerializerSettings GetJsonSettings()
    {
        return new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = { new Vector3Converter() },
            NullValueHandling = NullValueHandling.Ignore,
            //ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }

    public bool SaveData<T>(string ReletivePath, T Data)
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
            File.WriteAllText(path, JsonConvert.SerializeObject(Data, GetJsonSettings()));
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    public T LoadData<T>(string ReletivePath)
    {
        string path = Application.persistentDataPath + ReletivePath;

        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot load file at {path}. File does not exist.");
            throw new FileNotFoundException($"{path} does not exist.");
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path), GetJsonSettings());
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw e;
        }
    }

    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            Debug.Log("Vector3 ovveride saving called");
            JObject obj = new JObject
            {
                { "x", value.x },
                { "y", value.y },
                { "z", value.z }
            };

            obj.WriteTo(writer);
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            Debug.Log("Vector3 ovveride loading called");

            JObject obj = JObject.Load(reader);

            return new Vector3(
                (float)obj["x"],
                (float)obj["y"],
                (float)obj["z"]
            );
        }
    }
}
