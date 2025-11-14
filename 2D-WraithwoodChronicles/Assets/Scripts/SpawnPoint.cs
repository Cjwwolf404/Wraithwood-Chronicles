using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private GameObject spawnPoint;
    public string spawnID;
    private bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange)
        {
            if (Input.GetMouseButtonDown(1))
            {
                GameManager.Instance.ChangeSpawnPoint(this);

                UIManager.Instance.ChangeGamePromptText("The game has been saved");
                GameManager.Instance.SerializeJson();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            UIManager.Instance.ChangeGamePromptText("Right-click to save the game");
            UIManager.Instance.ChangeGamePromptPanelActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            UIManager.Instance.ChangeGamePromptPanelActive(true);
        }
    }
}
