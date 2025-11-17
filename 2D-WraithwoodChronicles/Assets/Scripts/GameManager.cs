using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject player;

    private IDataService DataService = new JsonDataService();

    [Header("Spawn Points")]

    public List<SpawnPoint> playerSpawnPoints;
    public SpawnPoint beginningSpawn;

    [Header("Player Status")]
    public SpawnPoint currentSpawnPoint;
    public int currentCurseEnergyAmount;
    public bool hasClawAbility;
    public bool hasClingAbility;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //player = GameObject.FindGameObjectWithTag("Player");

        SpawnPoint[] foundSpawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach(var sp in foundSpawnPoints)
        {
            playerSpawnPoints.Add(sp);
        }
    }

    public IEnumerator SetupNewGame()
    {
        yield return new WaitUntil (() => Instance != null);
        Debug.Log("Setting up new game");
        foreach (var sp in playerSpawnPoints)
        {
            if (sp.spawnID == "BeginningSpawn")
            {
                currentSpawnPoint = sp;
                break;
            }
        }

        currentCurseEnergyAmount = 0;
        hasClawAbility = false;
        hasClingAbility = false;

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (currentSpawnPoint != null)
        {
            player.transform.position = currentSpawnPoint.transform.position;
        }
    }

    public void ChangeSpawnPoint(SpawnPoint spawnPoint)
    {
        currentSpawnPoint = spawnPoint;
    }

    public void SerializeJson()
    {
        SaveState saveState = new SaveState
        {
            CurrentSpawnPointID = currentSpawnPoint.spawnID,
            CurrentCurseEnergyAmount = currentCurseEnergyAmount,
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
            StartCoroutine(ApplySaveData(data));
        }
        catch (Exception e)
        {
            Debug.LogError("Could not read save data file." + e.Message);
        }
    }

    private IEnumerator ApplySaveData(SaveState data)
    {
        //Wait for scene to load
        yield return null;

        foreach(var sp in playerSpawnPoints)
        {
            if(sp.spawnID == data.CurrentSpawnPointID)
            {
                currentSpawnPoint = sp;
                break;
            }
        }

        currentCurseEnergyAmount = data.CurrentCurseEnergyAmount;
        UIManager.Instance.UpdateCurseEnergyAmount();

        hasClawAbility = data.HasClawAbility;
        hasClingAbility = data.HasClingAbility;

        SpawnPlayer();
    }
    public class SaveState
    {
        public string CurrentSpawnPointID;
        public int CurrentCurseEnergyAmount;
        public bool HasClawAbility;
        public bool HasClingAbility;
    }
}

