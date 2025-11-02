using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(NPCDialogue))]
public class Scylus : MonoBehaviour
{
    [Header("Status")]
    public bool isInfected = true;
    public Transform scylusSpawnPoint;

    [Header("Dialogue Lines")]
    [TextArea(3, 10)]
    public List<string> dialogueLines1;
    [TextArea(3, 10)]
    public List<string> dialogueLines2;
    public bool firstInteraction = true;

    [Header("Infection Particles")]
    public ParticleSystem infectionGlowPrefab;
    private ParticleSystem instantiatedInfectionGlowPrefab;
    public ParticleSystem clearingInfectionPrefab;

    [Header("Player References")]
    public GameObject player;
    public PlayerController playerController;
    private bool playerInRange = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        instantiatedInfectionGlowPrefab = Instantiate(infectionGlowPrefab, new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(1))
        {
            playerController.DisableMovement();
            UIManager.Instance.ChangeGamePromptPanelActive(true);
            StartCoroutine(FirstScylusInteraction());
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!GetComponent<NPCDialogue>().isTalking)
            {
                if (firstInteraction)
                {
                    GetComponent<NPCDialogue>().StartDialogue(dialogueLines1);
                }
                else
                {
                    GetComponent<NPCDialogue>().StartDialogue(dialogueLines2);
                }
            }
            else
            {
                if (firstInteraction)
                {
                    GetComponent<NPCDialogue>().ContinueDialogue(dialogueLines1, true);
                }
                else
                {
                    GetComponent<NPCDialogue>().ContinueDialogue(dialogueLines2, false);
                }
            }
        }
    }

    IEnumerator FirstScylusInteraction()
    {
        while (Vector3.Distance(instantiatedInfectionGlowPrefab.transform.position, player.transform.position) > 0.1f)
        {
            instantiatedInfectionGlowPrefab.transform.position = Vector3.MoveTowards(instantiatedInfectionGlowPrefab.transform.position, player.transform.position, 3f * Time.deltaTime);
            yield return null;
        }

        instantiatedInfectionGlowPrefab.Stop();
        Destroy(instantiatedInfectionGlowPrefab.gameObject, 1f);

        Instantiate(clearingInfectionPrefab, player.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(2f);

        StartCoroutine(UIManager.Instance.FadeInBlackScreen());

        yield return new WaitForSeconds(2f);

        UIManager.Instance.SetupAbilityScreen("Claw Ability Obtained", "Left-click to slash enemies");
        UIManager.Instance.ChangeAbilityScreenActive(false);
        StartCoroutine(UIManager.Instance.AbilityGainedScreen());

        yield return new WaitForSeconds(4.5f);

        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        UIManager.Instance.ChangeAbilityScreenActive(true);
        StartCoroutine(UIManager.Instance.FadeOutBlackScreen());

        animator.SetBool("isInfected", false);
        isInfected = false;

        yield return new WaitForSeconds(2f);

        animator.SetTrigger("standUp");

        yield return new WaitForSeconds(3f);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = true;
    }

    public IEnumerator ScylusTeleport()
    {
        playerController.DisableMovement();

        StartCoroutine(UIManager.Instance.FadeInBlackScreen());

        yield return new WaitForSeconds(2f);

        transform.position = scylusSpawnPoint.position;
        GetComponent<Collider2D>().enabled = true;

        yield return new WaitForSeconds(1f);

        StartCoroutine(UIManager.Instance.FadeOutBlackScreen());

        yield return new WaitForSeconds(1f);

        GameManager.Instance.hasClawAbility = true;

        playerController.EnableMovement();
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInfected && collision.CompareTag("Player"))
        {
            playerInRange = true;
            UIManager.Instance.ChangeGamePromptText("Right-click to Absorb the Infection");
            UIManager.Instance.ChangeGamePromptPanelActive(false);
        }

        if (!isInfected && collision.CompareTag("Player"))
        {
            playerInRange = true;
            GetComponent<NPCDialogue>().dialoguePromptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (isInfected && collision.CompareTag("Player"))
        {
            playerInRange = false;
            UIManager.Instance.ChangeGamePromptPanelActive(true);
        }

        if (!isInfected && collision.CompareTag("Player"))
        {
            playerInRange = false;
            GetComponent<NPCDialogue>().isTalking = false;
            GetComponent<NPCDialogue>().dialoguePromptUI.SetActive(false);
        }
    }
}
