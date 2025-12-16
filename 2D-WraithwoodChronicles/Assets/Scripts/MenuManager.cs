using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public GameObject mainCanvas;
    public CanvasGroup canvasGroup;
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public CanvasGroup deathScreenCanvasGroup;

    public static bool gameIsPaused;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(SceneManager.GetActiveScene().name != "MainLevel")
        {
            canvasGroup.alpha = 0f;
            StartCoroutine (FadeIn(.5f));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "MainMenu" && !deathMenu.activeInHierarchy)
        {
            if(gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void NewGame()
    {
        StartCoroutine(FadeOut(0.8f, "GameIntroduction", false));
    }

    public void BeginNewGame()
    {
        StartCoroutine(FadeOut(0.8f, "MainLevel", true));
    }

    public void LoadGame()
    {
        StartCoroutine (FadeOut(0.8f, "MainLevel", false));
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene("MainLevel");
        GameManager.Instance.DeserializeJson();
    }

    public void PauseGame()
    {
        mainCanvas.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        PlayerController.Instance.enabled = false;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        mainCanvas.SetActive(true);
        Time.timeScale = 1f;
        gameIsPaused = false;
        PlayerController.Instance.enabled = true;
    }

    public void FadeInDeathScreen()
    {
        float fadeDuration = 1f;
        mainCanvas.SetActive(false);
        deathMenu.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            deathScreenCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            timer += Time.deltaTime;
        }
        deathScreenCanvasGroup.alpha = 1f;
    }

    public void SaveGame()
    {
        GameManager.Instance.SerializeJson();
        Debug.Log("Game Saved");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        mainCanvas.SetActive(true);
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void PlayButtonClickSound()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
    }

    public IEnumerator FadeIn(float fadeDuration)
    {
        canvasGroup.alpha = 0f;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeOut(float fadeDuration, string sceneToLoad, bool isStartingNewGame)
    {
        canvasGroup.alpha = 1f;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        SceneManager.LoadScene(sceneToLoad);

        if(sceneToLoad == "MainLevel")
        {
            if(isStartingNewGame)
            {
                GameManager.Instance.StartSetupNewGameCoroutine();
            }
            else
            {
                GameManager.Instance.DeserializeJson();
            }
        }
    }
}
