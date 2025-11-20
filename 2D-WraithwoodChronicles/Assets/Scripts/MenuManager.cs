using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public GameObject mainCanvas;
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public CanvasGroup deathScreenCanvasGroup;
    public float fadeDuration;

    public static bool gameIsPaused;

    void Awake()
    {
        Instance = this;
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

    public void PlayButtonClickSound()
    {
        AudioManager.Instance.PlaySound("ButtonClick");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainLevel");
        GameManager.Instance.SetupNewGame();
    }

    public void LoadGame()
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
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        mainCanvas.SetActive(true);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void FadeInDeathScreen()
    {
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
}
