using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    public float maxSpeed;
    public float jumpTakeOffSpeed;

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
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        targetVelocity = move * maxSpeed;
    }


}
