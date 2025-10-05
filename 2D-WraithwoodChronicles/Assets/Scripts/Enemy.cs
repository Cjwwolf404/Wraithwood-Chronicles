using System.Collections;
using System.Collections.Generic;
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

    private Transform player;
    private Vector2 currentTarget;
    private bool isChasing = false;

    private Vector2 patrolPointSize = new Vector2(0.5f, 0.5f);

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        currentTarget = pointB.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        animator = GetComponent<Animator>();
        currentHealth = enemyHealth;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
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

    public void AttackPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    public void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
        {
            currentTarget = currentTarget == pointA.position ? pointB.position : pointA.position;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<PlayerController>().TakeDamage(enemyDamage);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(pointA.position, patrolPointSize);
        Gizmos.DrawWireCube(pointB.position, patrolPointSize);
    }
}
