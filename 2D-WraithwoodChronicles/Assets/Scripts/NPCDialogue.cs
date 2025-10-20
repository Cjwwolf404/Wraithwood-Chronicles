using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Scylus))]
public class NPCDialogue : MonoBehaviour
{
    [TextArea(3, 10)]
    public List<string> dialogueLines;
    public string npcName;

    private bool playerInRange = false;
    private int currentLine = 0;
    private bool isTalking = false;

    public GameObject dialoguePromptUI;

    public DialogueUI dialogueUI;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                StartDialogue();
            }
            else
            {
                ContinueDialogue();
            }
        }
    }

    public void StartDialogue()
    {
        isTalking = true;
        currentLine = 0;
        dialoguePromptUI.SetActive(false);
        dialogueUI.ShowDialogue(npcName, dialogueLines[currentLine]);
    }

    public void ContinueDialogue()
    {
        currentLine++;

        if (currentLine < dialogueLines.Count)
        {
            dialogueUI.ShowDialogue(npcName, dialogueLines[currentLine]);
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        isTalking = false;
        dialoguePromptUI.SetActive(true);
        dialogueUI.HideDialogue();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GetComponent<Scylus>().isInfected && collision.CompareTag("Player"))
        {
            playerInRange = true;
            dialoguePromptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            isTalking = false;
            dialoguePromptUI.SetActive(false);
        }
    }
}
