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
    public float verticalTolerance;

    private Transform player;
    private Vector3 currentTarget;
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
        float verticalDistance = Mathf.Abs(transform.position.y - player.position.y);

        if (distanceToPlayer <= detectionRange && verticalDistance <= verticalTolerance)
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

    public void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentTarget, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
        {
            currentTarget = currentTarget == pointA.position ? pointB.position : pointA.position;
        }
    }

    public void AttackPlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(enemyDamage);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(pointA.position, patrolPointSize);
        Gizmos.DrawWireCube(pointB.position, patrolPointSize);
    }
}
