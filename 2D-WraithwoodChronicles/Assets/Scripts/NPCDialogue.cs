using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Scylus))]
public class NPCDialogue : MonoBehaviour
{
    public string npcName;

    private int currentLine = 0;
    public bool isTalking = false;

    public GameObject dialoguePromptUI;

    public DialogueUI dialogueUI;

    public void StartDialogue(List<string> dialogueLines)
    {
        isTalking = true;
        currentLine = 0;
        dialoguePromptUI.SetActive(false);
        dialogueUI.ShowDialogue(npcName, dialogueLines[currentLine]);
    }

    public void ContinueDialogue(List<string> dialogueLines, bool firstInteraction)
    {
        currentLine++;

        if (currentLine < dialogueLines.Count)
        {
            dialogueUI.ShowDialogue(npcName, dialogueLines[currentLine]);
        }
        else
        {
            EndDialogue();
            if(npcName == "Scylus" && firstInteraction)
            {
                GetComponent<Scylus>().GetComponent<Collider2D>().enabled = false;
                GetComponent<Scylus>().firstInteraction = false;
                StartCoroutine(GetComponent<Scylus>().ScylusTeleport());
            }
        }
    }

    public void EndDialogue()
    {
        isTalking = false;
        dialoguePromptUI.SetActive(true);
        dialogueUI.HideDialogue();
    }
}
