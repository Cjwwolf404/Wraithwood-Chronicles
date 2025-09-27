using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    public PlayerController playerController;

    public void ShowDialogue(string npcName, string line)
    {
        dialoguePanel.SetActive(true);
        nameText.text = npcName;
        dialogueText.text = line;
        playerController.DisableMovement();

    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
        playerController.EnableMovement();
    }
}
