using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject player;
    public PlayerController playerController;

    private IDataService DataService = new JsonDataService();
    
    public List<GameObject> playerSpawnPoints;

    public GameObject currentSpawnPoint;
    public bool hasClawAbility;
    public bool hasClingAbility;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(currentSpawnPoint != null)
        {
            player.transform.position = currentSpawnPoint.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeSpawnPoint(int index)
    {
        currentSpawnPoint = playerSpawnPoints[index];
    }

    public void RespawnPlayer()
    {
        playerController.currentHealth = playerController.maxPlayerHealth;
        player.transform.position = currentSpawnPoint.transform.position;
    }

    public void SerializeJson()
    {
        var saveState = new SaveState
        {
            CurrentSpawnPoint = currentSpawnPoint,
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
        currentSpawnPoint = data.CurrentSpawnPoint;
        hasClawAbility = data.HasClawAbility;
        hasClingAbility = data.HasClingAbility;
    }
    public class SaveState
    {
        public GameObject CurrentSpawnPoint;
        public bool HasClawAbility;
        public bool HasClingAbility;
    }
}

