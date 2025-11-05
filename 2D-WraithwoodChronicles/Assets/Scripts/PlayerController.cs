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

    [Header("Health, Damage, and Curse Energy")]
    public float maxPlayerHealth;
    public float currentHealth;
    public float playerDamage;
    public float knockbackForce;
    public float curseEnergyAmount;

    [Header("Attack Circle")]
    public GameObject attackPoint;
    public float attackRadius;
    public LayerMask enemies;
    private float attackWait = 0.6f;
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

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxPlayerHealth;
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        Gravity();

        GroundCheck();
        
        FlipSprite();

        if(GameManager.Instance.hasClawAbility)
        {
            if (Input.GetMouseButtonDown(0) && canMove && !isAttacking)
            {
                isAttacking = true;
                lastAttackTime = Time.time;
                animator.SetTrigger("startAttack");
                StartCoroutine(AttackCombo());
            }
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

            if(isAttacking)
            {
                animator.SetBool("isJumping", false);
            }
        }

        if (rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", true);

            if(isAttacking)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", false);
            }
        }

        if(currentHealth <= 0)
        {
            canMove = false;
            StartCoroutine(UIManager.Instance.FadeInBlackScreen());
            UIManager.Instance.FadeInDeathScreen();
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
    }

    public IEnumerator AttackCombo()
    {
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
                    secondSwing = true;
                }
                else
                {
                    lastAttackTime = Time.time;
                    animator.SetBool("secondSwing", false);
                    secondSwing = false;
                }
            }

            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetButtonDown("Jump"))
            {
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        animator.SetBool("secondSwing", false);
        animator.SetTrigger("endAttack");
        isAttacking = false;
    }

    public void TakeDamage(float damage, Vector2 sourcePosition, float knockbackForce)
    {
        currentHealth -= damage;
        UIManager.Instance.UpdateHealthBar(currentHealth);

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
