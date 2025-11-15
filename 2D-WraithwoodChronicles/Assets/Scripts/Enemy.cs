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
    private Vector2 patrolPointSize = new Vector2(0.5f, 0.5f);
    private Transform currentTarget;

    [Header("Attacking")]
    public float chaseSpeed;
    public float detectionRange;
    public float knockbackForce;
    public PlayerController playerController;
    private Transform player;

    [Header("Edge Detection")]
    public Transform groundCheck;
    public float groundCheckDistance;
    public float flipCooldown;
    private float lastFlipTime;
    public Vector2 groundCheckSize;
    public LayerMask groundLayer;

    private Rigidbody2D rb;

    private bool isChasing = false;
    private bool canMove = true;
    private bool isKnockedBack = false;
    private bool isFacingRight = false;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentTarget = pointA;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentHealth = enemyHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (canMove && !isKnockedBack)
        {
            if (distanceToPlayer <= detectionRange)
            {
                isChasing = true;
            }
            else
            {
                isChasing = false;
            }

            if (!IsGroundAhead())
            {
                if (isChasing)
                {
                    StopAtEdge();
                    return;
                }

                if (Time.time - lastFlipTime > flipCooldown)
                {
                    FlipEnemy();
                }
            }

            MoveEnemy();
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

    private void MoveEnemy()
    {
        float verticalVelocity = rb.velocity.y;
        float speed = isChasing ? chaseSpeed : patrolSpeed;

        float direction = 0f;

        if (isChasing) //Attacking player state
        {
            animator.SetBool("isAttacking", true);
            float relativePosition = player.position.x - transform.position.x;
            direction = Mathf.Sign(relativePosition);

            if ((relativePosition > 0 && !isFacingRight) || (relativePosition < 0 && isFacingRight))
            {
                FlipEnemy();
            }
        }
        else //Patrolling state
        {
            animator.SetBool("isAttacking", false);
            direction = Mathf.Sign(currentTarget.position.x - transform.position.x);
            if (Mathf.Abs(transform.position.x - currentTarget.position.x) <= 0.1f)
            {
                FlipEnemy();
            }
        }

        rb.velocity = new Vector2(direction * speed, verticalVelocity);
    }
    
    private bool IsGroundAhead()
    {
        Vector2 origin = groundCheck.position + Vector3.right * (isFacingRight ? groundCheckDistance : groundCheckDistance);
        bool hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(origin, Vector2.down * groundCheckDistance, hit ? Color.green : Color.red);
        return hit;
    }

    public void FlipEnemy()
    {
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
        isFacingRight = !isFacingRight;
        Vector3 ls = transform.localScale;
        ls.x *= -1;
        transform.localScale = ls;
        lastFlipTime = Time.time;
    }
    
    public void StopAtEdge()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision) //Attack
    {
        while (collision.gameObject.CompareTag("Player") && playerController.canTakeDamage)
        {
            Debug.Log("Attacking");
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(enemyDamage, transform.position, knockbackForce);
            StartCoroutine(AttackCooldown());
        }
    }

    IEnumerator AttackCooldown()
    {
        DisableMovement();
        animator.SetBool("canMove", false);

        yield return new WaitForSeconds(0.5f);

        EnableMovement();
        animator.SetBool("canMove", true);

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

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
