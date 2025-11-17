using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstEnemyType : Enemy
{
    // Update is called once per frame
    void Update()
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

        // if (currentHealth < enemyHealth)
        // {
        //     currentHealth = enemyHealth;
        //     animator.SetTrigger("Attacked");
        // }

        if (currentHealth <= 0)
        {
            for(int i = 0; i < dropAmount; i++)
            {
                Instantiate(curseEnergyPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    public void MoveEnemy()
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
}
