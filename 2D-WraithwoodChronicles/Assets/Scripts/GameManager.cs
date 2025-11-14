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

    public List<SpawnPoint> playerSpawnPoints;

    private SpawnPoint beginningSpawn;
    public SpawnPoint currentSpawnPoint;
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

        player = GameObject.FindGameObjectWithTag("Player");

        SpawnPoint[] foundSpawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach(var sp in foundSpawnPoints)
        {
            playerSpawnPoints.Add(sp);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SpawnPlayer()
    {
        Debug.Log("Spawning player");
        player = GameObject.FindGameObjectWithTag("Player");

        if (currentSpawnPoint != null)
        {
            player.transform.position = currentSpawnPoint.transform.position;
        }
        // else
        // {
        //     currentSpawnPoint = beginningSpawn;
        //     player.transform.position = beginningSpawn.transform.position;
        // }
    }

    public void ChangeSpawnPoint(SpawnPoint spawnPoint)
    {
        currentSpawnPoint = spawnPoint;
    }

    public void PlayerDeath()
    {
        StartCoroutine(UIManager.Instance.FadeInBlackScreen());
        //UIManager.Instance.FadeInDeathScreen();
    }

    public void RespawnPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //player.transform.position = currentSpawnPoint.transform.position;
    }

    public void SerializeJson()
    {
        SaveState saveState = new SaveState
        {
            CurrentSpawnPointID = currentSpawnPoint.spawnID,
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

        Debug.Log("applying save data..." + JsonConvert.SerializeObject(data, Formatting.Indented));

        foreach(var sp in playerSpawnPoints)
        {
            if(sp.spawnID == data.CurrentSpawnPointID)
            {
                currentSpawnPoint = sp;
                break;
            }
        }

        //currentSpawnPoint.position = data.CurrentSpawnPoint;
        hasClawAbility = data.HasClawAbility;
        hasClingAbility = data.HasClingAbility;
        Debug.Log(hasClawAbility + "" + hasClingAbility + " " + currentSpawnPoint.spawnID);

        SpawnPlayer();
    }
    public class SaveState
    {
        // [JsonConverter(typeof(JsonDataService.Vector3Converter))]
        // public Vector3 CurrentSpawnPoint;

        public string CurrentSpawnPointID;
        
        public bool HasClawAbility;
        public bool HasClingAbility;
    }
}

