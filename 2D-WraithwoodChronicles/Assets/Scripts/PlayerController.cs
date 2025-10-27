using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    public float maxSpeed;
    float horizontalInput;
    public float jumpTakeOffSpeed;
    public int maxJumps;
    private int jumpsRemaining;

    [Header("Health and Damage")]
    public float playerHealth;
    private float currentHealth;
    public float playerDamage;
    public float knockbackForce;

    [Header("Attack Circle")]
    public GameObject attackPoint;
    public float attackRadius;
    public LayerMask enemies;
    private float attackWait = 1f;
    private float lastAttackTime;
    private bool isAttacking = false;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity;
    public float maxFallSpeed;
    public float fallSpeedMultiplier;

    private bool isFacingRight = true;
    private bool canMove = true;
    private bool isKnockedBack = false;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        Gravity();

        GroundCheck();
        
        FlipSprite();

        if (Input.GetMouseButtonDown(0) && canMove && !isAttacking)
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            animator.SetTrigger("startAttack");
            Attack();
            StartCoroutine(AttackCombo());
        }

        if (jumpsRemaining > 0 && canMove)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpTakeOffSpeed);
                    jumpsRemaining--;
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    if (rb.velocity.y > 0)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                        jumpsRemaining--;
                    }
                }
            }

        if (rb.velocity.y > 0)
        {
            animator.SetBool("isJumping", true);
        }

        if (rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", true);
        }

        if(playerHealth <= 0)
        {
            Debug.Log("Player died");
        }
    }

    private void FixedUpdate()
    {
        if (canMove && !isKnockedBack)
        {
            rb.velocity = new Vector2(horizontalInput * maxSpeed, rb.velocity.y);
            animator.SetFloat("xVelocity", Math.Abs(rb.velocity.x));
            animator.SetFloat("yVelocity", rb.velocity.y);
        }

        if(isAttacking)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                StopCoroutine(AttackCombo());
                animator.SetBool("secondSwing", false);
                animator.SetBool("isAttacking", false);
                isAttacking = false;
                Debug.Log("Attack cancelled");
            }
        }
    }

    private void Gravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            jumpsRemaining = maxJumps;
        }
    }

    public void Attack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, enemies);

        foreach (Collider2D enemyGameObject in enemy)
        {
            enemyGameObject.GetComponent<Enemy>().TakeDamage(playerDamage, transform.position, knockbackForce);
        }

        Debug.Log("Attack");
    }

    public IEnumerator AttackCombo()
    {
        Debug.Log("Coroutine starting");
        bool secondSwing = false;
        yield return new WaitForSeconds(0.05f);

        while (Time.time - lastAttackTime < attackWait)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!secondSwing)
                {
                    lastAttackTime = Time.time;
                    animator.SetBool("secondSwing", true);
                    Attack();
                    secondSwing = true;
                    Debug.Log("Second swing");
                }
                else
                {
                    lastAttackTime = Time.time;
                    animator.SetBool("secondSwing", false);
                    Attack();
                    secondSwing = false;
                    Debug.Log("First swing");
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        animator.SetBool("secondSwing", false);
        animator.SetTrigger("endAttack");
        isAttacking = false;
        Debug.Log("Coroutine finished");
    }

    public void TakeDamage(float damage, Vector2 sourcePosition, float knockbackForce)
    {
        playerHealth -= damage;

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

    void FlipSprite()
    {
        if (canMove)
        {
            if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRadius);
    }
}
