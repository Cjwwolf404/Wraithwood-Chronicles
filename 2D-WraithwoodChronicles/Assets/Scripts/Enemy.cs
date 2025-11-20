using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Attack and Damage")]
    public float enemyHealth;
    private protected float currentHealth;
    public float enemyDamage;
    public int dropAmount;
    private protected bool canGiveDamage;

    [Header("Patrolling")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed;
    private Vector2 patrolPointSize = new Vector2(0.5f, 0.5f);
    private protected Transform currentTarget;

    [Header("Attacking")]
    public float chaseSpeed;
    public float detectionRange;
    public float knockbackForce;
    public PlayerController playerController;
    private protected Transform player;

    [Header("Edge Detection")]
    public Transform groundCheck;
    private protected float groundCheckDistance = 0.4f;
    private protected float flipCooldown = 0.5f;
    private protected float lastFlipTime;
    private protected Vector2 groundCheckSize = new(0.5f, 0.5f);
    private protected LayerMask groundLayer;

    private protected Rigidbody2D rb;

    private protected bool isChasing = false;
    private protected bool canMove = true;
    private protected bool isKnockedBack = false;
    private protected bool isFacingRight = false;

    [Header("Particle System Prefab")]
    public GameObject curseEnergyPrefab;

    private protected Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        groundLayer = LayerMask.GetMask("Ground");

        currentTarget = pointA;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentHealth = enemyHealth;
        canGiveDamage = true;
    }
    
    public bool IsGroundAhead()
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

    void OnCollisionStay2D(Collision2D collision) //Attack
    {
        if(!canGiveDamage) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if(playerController.canTakeDamage)
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(enemyDamage, transform.position, knockbackForce);
                StartCoroutine(AttackCooldown(0.5f));
            }
        }
    }

    protected IEnumerator AttackCooldown(float time)
    {
        DisableMovement();

        yield return new WaitForSeconds(time);

        EnableMovement();
        yield return null;
    }

    public void TakeDamage(float playerDamage, Vector2 sourcePosition, float knockbackForce)
    {
        AudioManager.Instance.PlaySound("EnemyDamage", gameObject);

        currentHealth -= playerDamage;

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
        animator.SetBool("canMove", false);
    }

    public void EnableMovement()
    {
        canMove = true;
        animator.SetBool("canMove", true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(pointA.position, patrolPointSize);
        Gizmos.DrawWireCube(pointB.position, patrolPointSize);

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
