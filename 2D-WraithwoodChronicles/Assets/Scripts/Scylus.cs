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

    [Header("Audio Clips")]
    public List<AudioClip> idleSounds;
    private float minTimeBetweenSounds = 5f;
    private float maxTimeBetweenSounds = 15f;
    public List<AudioClip> talkSounds;
    private AudioSource audioSource;

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
    public GameObject playerBoundaryMV;
    private bool playerInRange = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
                    PlaySound("ScylusGreeting");
                }
                else
                {
                    GetComponent<NPCDialogue>().StartDialogue(dialogueLines2);
                    PlaySound("ScylusGreeting");
                }
            }
            else
            {
                double random = Random.Range(0f, 1f);
                if (firstInteraction)
                {
                    GetComponent<NPCDialogue>().ContinueDialogue(dialogueLines1, true);
                    if(random < 0.5f && GetComponent<NPCDialogue>().currentLine != dialogueLines1.Count)
                    {
                        AudioClip clip = talkSounds[Random.Range(1, talkSounds.Count)];
                        audioSource.PlayOneShot(clip);
                    }
                }
                else
                {
                    GetComponent<NPCDialogue>().ContinueDialogue(dialogueLines2, false);
                    if(random < 0.5f && GetComponent<NPCDialogue>().currentLine != dialogueLines2.Count)
                    {
                        AudioClip clip = talkSounds[Random.Range(1, talkSounds.Count)];
                        audioSource.PlayOneShot(clip);
                    }
                }
            }
        }
    }

    public IEnumerator FirstScylusInteraction()
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

        playerBoundaryMV.SetActive(false);

        playerController.EnableMovement();

        StartCoroutine(PlayIdleSounds());
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

    public IEnumerator PlayIdleSounds()
    {
        while(true)
        {
            if(!GetComponent<NPCDialogue>().isTalking)
            {
                float waitTime = Random.Range(minTimeBetweenSounds,maxTimeBetweenSounds);
                yield return new WaitForSeconds(waitTime);

                if(GetComponent<NPCDialogue>().isTalking)
                {
                    yield return new WaitUntil(() => !GetComponent<NPCDialogue>().isTalking);
                    continue;
                }

                AudioClip randomSound = idleSounds[Random.Range(0, idleSounds.Count)];

                audioSource.PlayOneShot(randomSound);
            }
        }
    }

    public void PlaySound(string clipName)
    {
        AudioClip clip = talkSounds.Find(c => c.name == clipName);

        audioSource.PlayOneShot(clip);
    }
}
