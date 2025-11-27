using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondEnemyType : Enemy
{
    [Header("Second Enemy Variables")]
    public float attackRange;
    public float retreatRange;
    public float retreatSpeed;
    private bool isRetreating;
    private float lastAttackTime = -9999f;
    public GameObject sludgeBallPrefab;
    public Transform sludgeBallSpawnPoint;

    void Update()
    {
        cancelAttackCooldown = false;
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

            if (distanceToPlayer < retreatRange)
            {
                isRetreating = true;
                cancelAttackCooldown = true;
            }
            else
            {
                isRetreating = false;
            }

            if (distanceToPlayer < attackRange && distanceToPlayer > retreatRange) //Shooting sludge state
            {
                //if(Time.deltaTime > lastAttackTime + shootCooldown)
                {
                    ShootSludgeBall();
                    return;
                }
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

        if (currentHealth <= 0)
        {
            for(int i = 0; i < dropAmount; i++)
            {
                Instantiate(curseEnergyPrefab, transform.position, Quaternion.identity);
            }
            Instantiate(deathBloodSplat, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void MoveEnemy()
    {
        float verticalVelocity = rb.velocity.y;
        float speed;

        if(isRetreating)
        {
            speed = retreatSpeed;
        }
        else
        {
            speed = isChasing ? chaseSpeed : patrolSpeed;
        }

        float direction = 0f;

        if (isChasing) //Chasing state
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

        if(isRetreating)
        {
            rb.velocity = new Vector2(-direction * speed, verticalVelocity);         
        }
        else
        {
            rb.velocity = new Vector2(direction * speed, verticalVelocity);
        }
    }

    public void ShootSludgeBall()
    {
        lastAttackTime = Time.deltaTime;
        
        animator.SetTrigger("shoot");

        rb.velocity = new Vector2(0f, rb.velocity.y);

        StartCoroutine(AttackCooldown(2));
    }

    public void InstantiateSludgeBall()
    {
        Instantiate(sludgeBallPrefab, sludgeBallSpawnPoint.position, Quaternion.identity);
    }
}
