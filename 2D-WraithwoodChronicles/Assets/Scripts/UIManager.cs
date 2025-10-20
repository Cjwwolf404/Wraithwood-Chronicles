using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject gamePromptPanel;

    public TMP_Text gamePromptText;

    private void Awake()
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

    public void ChangeGamePromptText(string newText)
    {
        gamePromptText.text = newText;
    }

    public void ChangeGamePromptPanelActive(bool isActive)
    {
        if (isActive)
        {
            gamePromptPanel.SetActive(false);
        }
        else
        {
            gamePromptPanel.SetActive(true);
        }
    }
}
