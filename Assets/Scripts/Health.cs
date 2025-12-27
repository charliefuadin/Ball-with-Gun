using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private Rigidbody2D rb;
    public int maxHealth = 100;
    public int currentHealth;
    public string[] collisionList;
    public ParticleSystem deathParticle;

    public SpriteRenderer sprite;
    public Color currentSprite;

    public float gloablKnockPower; //Remove later

    private EnemyMovement enemyMovement;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyMovement = GetComponent<EnemyMovement>();
        currentHealth = maxHealth;
        currentSprite = sprite.color;
    }

    private void Update()
    {   //Enemy Death
        if(currentHealth <= 0)
        {
            Instantiate(deathParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    //GitHub Test
    IEnumerator Flash()
    {   //Flash when hit
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sprite.color = currentSprite;
    }

    public void enemyHit(Vector2 direction, float knockPower) //add a parameter for damageTaken
    {
        StartCoroutine(Flash());
        anim.SetTrigger("damageTaken");

        //Sets the knock back trigger also set in EnemyMovement
        enemyMovement.previousState = enemyMovement.currentState;
        enemyMovement.currentState = EnemyStates.Knockback;
        enemyMovement.knockbackDuration = 0.2f;

        rb.AddForce(direction * knockPower, ForceMode2D.Impulse); //Physics Knockback
    }

    //Remove this ontrigger enter later and replace it into the bullet movement code
    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < collisionList.Length; i++)
        {
            if (collision.gameObject.CompareTag(collisionList[i]))
            {   //Based on the bullet prefab
                Vector2 direction = (transform.position - collision.gameObject.transform.position).normalized;
                enemyHit(direction, gloablKnockPower);
                currentHealth -= BulletMovement.instance.bulletDamage;
            }
        }
    }
}
