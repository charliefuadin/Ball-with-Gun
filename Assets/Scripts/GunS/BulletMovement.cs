using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public static BulletMovement instance;
    public int bulletDamage;

    public float speed = 100;
    public Rigidbody2D rb2D;
    public Transform bullet;
    public string[] collisionList;

    public ParticleSystem bulletParticle;
    public ParticleSystem enemyHitParticle;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        StartCoroutine(KillBullet());

    }
    void Update()
    {
        rb2D.velocity = -bullet.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {   
        foreach (string collider in collisionList)
        {
            if (collision.gameObject.CompareTag(collider))
            {
                if(collider == "Enemy")
                {
                    Health health = collision.gameObject.GetComponent<Health>();
                    if(health != null)
                    {
                        Vector2 direction = (collision.gameObject.transform.position - transform.position).normalized;
                        health.enemyHit(direction, health.gloablKnockPower, bulletDamage);
                    }
                }
                else
                {
                    Instantiate(bulletParticle, transform.position, transform.rotation);
                }
                Destroy(gameObject);
            }
        }
    }

    IEnumerator KillBullet()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
