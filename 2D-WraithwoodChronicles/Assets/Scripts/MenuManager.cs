using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject pauseMenu;

    public static bool gameIsPaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "MainMenu")
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

    public void PlayGame()
    {
        SceneManager.LoadScene("MainLevel");
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

    public void SaveGame()
    {
        GameManager.Instance.SerializeJson();
        Debug.Log("Game Saved");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
