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
                Vector3 pos = transform.position;
                Quaternion rotation = transform.rotation;

                if(collider == "Enemy" || collider =="Bullet")
                {
                    Instantiate(enemyHitParticle, pos, rotation); 
                }
                else
                {
                    Instantiate(bulletParticle, pos, rotation);
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
