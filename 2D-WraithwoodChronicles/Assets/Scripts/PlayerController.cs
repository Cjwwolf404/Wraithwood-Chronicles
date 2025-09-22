using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    public float maxSpeed;
    public float jumpTakeOffSpeed;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {

    }

    protected override void ComputeVelocity()
    {
        UnityEngine.Vector2 move = UnityEngine.Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpTakeOffSpeed;
            isGrounded = false;
            animator.SetBool("isJumping", !isGrounded);
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        targetVelocity = move * maxSpeed;
        animator.SetFloat("xVelocity", Math.Abs(targetVelocity.x));
        animator.SetFloat("yVelocity", targetVelocity.y);
    }

    // private void OnCollisionEnter2D(Collider2D collision)
    // {
    //     isGrounded = true;
    //     animator.SetBool("isJumping", !isGrounded);
    // }

}
