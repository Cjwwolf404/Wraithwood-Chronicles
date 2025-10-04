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
    public float playerDamage;

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

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        FlipSprite();

        Gravity();

        GroundCheck();

        if (Input.GetMouseButtonDown(0) && canMove)
        {
            Attack();
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
    }

    private void FixedUpdate()
    {
        if (canMove)
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

    public void Attack()
    {
        Debug.Log("Attack");
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
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
}
