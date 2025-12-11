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

    [Header("Game Status")]
    public SpawnPoint currentSpawnPoint;
    public int currentCurseEnergyAmount;
    public bool scylusTeleported;
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
    }

    public void StartSetupNewGameCoroutine()
    {
        StartCoroutine(SetupNewGame());
    }

    public IEnumerator SetupNewGame()
    {
        yield return null;

        SpawnPoint[] foundSpawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach(var sp in foundSpawnPoints)
        {
            playerSpawnPoints.Add(sp);
        }

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

        SerializeJson();

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        UIManager.Instance.blackScreenCanvasGroup.alpha = 1f;

        player = GameObject.FindGameObjectWithTag("Player");

        player.GetComponent<PlayerController>().DisableMovement();

        if (currentSpawnPoint != null)
        {
            player.transform.position = new Vector3(currentSpawnPoint.transform.position.x - 10, 
                                                    currentSpawnPoint.transform.position.y,
                                                    currentSpawnPoint.transform.position.z);
        }

        StartCoroutine(WaitForFade());
    }

    public IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(1);

        player.GetComponent<PlayerController>().EnableMovement();

        StartCoroutine(UIManager.Instance.FadeOutBlackScreen(3));

        StartCoroutine(UIManager.Instance.FadeInPlayerUI(1f));
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
            ScylusTeleported = scylusTeleported,
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

        playerSpawnPoints.Clear();

        SpawnPoint[] foundSpawnPoints = FindObjectsOfType<SpawnPoint>();
        foreach(var sp in foundSpawnPoints)
        {
            playerSpawnPoints.Add(sp);
        }

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

        scylusTeleported = data.ScylusTeleported;
        hasClawAbility = data.HasClawAbility;
        hasClingAbility = data.HasClingAbility;

        SpawnPlayer();
    }
    public class SaveState
    {
        public string CurrentSpawnPointID;
        public int CurrentCurseEnergyAmount;
        public bool ScylusTeleported;
        public bool HasClawAbility;
        public bool HasClingAbility;
    }
}

