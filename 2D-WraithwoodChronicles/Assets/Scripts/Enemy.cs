using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Attack and Damage")]
    public float enemyHealth;
    private float currentHealth;
    public float enemyDamage;

    [Header("Patrolling")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed;

    [Header("Attacking")]
    public float chaseSpeed;
    public float detectionRange;
    public float verticalTolerance;
    public float knockbackForce;

    private Transform player;
    private Transform currentTarget;
    private bool isChasing = false;

    private Vector2 patrolPointSize = new Vector2(0.5f, 0.5f);

    private Rigidbody2D rb;

    private bool canMove = true;

    private bool isKnockedBack = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentTarget = pointB;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        animator = GetComponent<Animator>();
        currentHealth = enemyHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float verticalDistance = Mathf.Abs(transform.position.y - player.position.y);

        if(canMove && !isKnockedBack)
        {
            if (distanceToPlayer <= detectionRange && verticalDistance <= verticalTolerance)
            {
                isChasing = true;
            }
            else
            {
                isChasing = false;
            }

            if (isChasing)
            {
                AttackPlayer();
            }
            else
            {
                Patrol();
            }
        }

        if (enemyHealth < currentHealth)
        {
            currentHealth = enemyHealth;
            animator.SetTrigger("Attacked");
        }

        if (enemyHealth <= 0)
        {
            Debug.Log("Enemy died");
            Destroy(gameObject);
        }
    }

    public void Patrol()
    {
        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = new Vector2(currentTarget.position.x, currentPosition.y);

        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, patrolSpeed * Time.deltaTime);
        rb.MovePosition(newPosition);

        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    public void AttackPlayer()
    {
        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = new Vector2(player.position.x, currentPosition.y);

        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, chaseSpeed * Time.deltaTime);
        rb.MovePosition(newPosition);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(enemyDamage, transform.position, knockbackForce);
            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        DisableMovement();

        yield return new WaitForSeconds(0.5f);

        EnableMovement();

        yield return null;
    }

    public void TakeDamage(float playerDamage, Vector2 sourcePosition, float knockbackForce)
    {
        enemyHealth -= playerDamage;

        if (isKnockedBack) return;

        isKnockedBack = true;

        float knockbackDirection = transform.position.x < sourcePosition.x ? -1 : 1;

        rb.velocity = Vector2.zero;

        rb.AddForce(new Vector2(knockbackDirection * knockbackForce, 3f), ForceMode2D.Impulse);

        Invoke(nameof(EndKnockback), 0.2f);
    }
    
    private void EndKnockback()
    {
        isKnockedBack = false;
    }

    public void DisableMovement()
    {
        canMove = false;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        //animator.SetBool("canMove", false);
    }

    public void EnableMovement()
    {
        canMove = true;
        //animator.SetBool("canMove", true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(pointA.position, patrolPointSize);
        Gizmos.DrawWireCube(pointB.position, patrolPointSize);
    }
}
