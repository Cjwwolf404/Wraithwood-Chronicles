using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Scylus : MonoBehaviour
{
    public bool isInfected = true;

    private bool playerInRange = false;

    public GameObject player;
    public PlayerController playerController;

    public ParticleSystem infectionGlowPrefab;
    private ParticleSystem instantiatedInfectionGlowPrefab;
    public ParticleSystem clearingInfectionPrefab;
    private bool isMoving;


    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        instantiatedInfectionGlowPrefab = Instantiate(infectionGlowPrefab, new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetMouseButtonDown(1))
        {
            playerController.DisableMovement();
            UIManager.Instance.ChangeGamePromptPanelActive(true);
            isMoving = true;
        }
        
        if (isMoving)
        {
            instantiatedInfectionGlowPrefab.transform.position = Vector3.MoveTowards(instantiatedInfectionGlowPrefab.transform.position, player.transform.position, 2f * Time.deltaTime);

            if (Vector3.Distance(instantiatedInfectionGlowPrefab.transform.position, player.transform.position) < 0.1f)
            {
                isMoving = false;
                instantiatedInfectionGlowPrefab.Stop();
                Destroy(instantiatedInfectionGlowPrefab.gameObject, 1f);

                ParticleSystem instantiatedClearingInfectionPrefab = Instantiate(clearingInfectionPrefab, player.transform.position, Quaternion.identity);
                animator.SetBool("isInfected", false);

                isInfected = false;
                StartCoroutine(ResetCollider());
            }
        }
    }

    IEnumerator ResetCollider()
    {
        yield return new WaitForSeconds(6.5f);

        playerController.EnableMovement();
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = true;

        yield return null;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInfected && collision.CompareTag("Player"))
        {
            playerInRange = true;
            UIManager.Instance.ChangeGamePromptText("Right Click to Absorb the Infection");
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
