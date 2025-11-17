using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SludgeBall : MonoBehaviour
{
    public float damage;
    public float xVelocity;
    public float yVelocity;
    public float knockbackForce;

    public GameObject sludgeExplodePrefab;

    private GameObject player;
    private Rigidbody2D rb;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        float relativePosition = player.transform.position.x - transform.position.x;
        float direction = Mathf.Sign(relativePosition);

        rb.AddForce(new Vector2(direction * xVelocity, yVelocity), ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<PlayerController>().TakeDamage(damage, transform.position, knockbackForce);
        }

        Instantiate(sludgeExplodePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
