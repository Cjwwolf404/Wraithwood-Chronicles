using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //private SaveState SaveState = new SaveState();
    private IDataService DataService = new JsonDataService();

    public bool hasClawAbility;
    public bool hasClingAbility;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SerializeJson()
    {
        var saveState = new SaveState
        {
            HasClawAbility = hasClawAbility,
            HasClingAbility = hasClingAbility,
        };

        if (!DataService.SaveData("/save-state.json", saveState))
        {
            Debug.LogError("Could not save file.");
        }
    }

    public void DeserializeJson()
    {
        try
        {
            SaveState data = DataService.LoadData<SaveState>("/save-state.json");
            ApplySaveData(data);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not read save data file." + e.Message);
        }
    }

    public void ApplySaveData(SaveState data)
    {
        Debug.Log("applying save data..." + JsonConvert.SerializeObject(data, Formatting.Indented));
        hasClawAbility = data.HasClawAbility;
        hasClingAbility = data.HasClingAbility;
        Debug.Log(hasClawAbility);
    }
    public class SaveState
    {
        public bool HasClawAbility { get; set; }
        public bool HasClingAbility { get; set; }
    }
}

