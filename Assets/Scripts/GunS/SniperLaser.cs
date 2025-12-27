using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperLaser : MonoBehaviour
{
    [SerializeField] ParticleSystem enemyHitParticle;
    [SerializeField] Animator anim;

    [SerializeField] Transform firePoint;
    [SerializeField] LineRenderer sniperLaser;

    private RaycastHit2D hit;
    private Vector2 direction;
    [SerializeField] int pierceAmount;

    private bool ableToClick = true;
    [SerializeField] float clickRate;


    private void Update()
    {
        GunDirection();
        Laser();
        Shoot();
    }

    private void Shoot()
    {
        //Semi-Auto shots based on players click
        if (Input.GetButtonUp("Fire1"))
        {
            if (ableToClick == true)
            {
                anim.SetBool("isShooting", true);
                SniperShot();
                ableToClick = false;
                StartCoroutine(clickCheck());
            }
        }
        else
        {
            anim.SetBool("isShooting", false);
        }
    }

    private IEnumerator clickCheck()
    {
        //Sets per click
        sniperLaser.enabled = false;
        yield return new WaitForSeconds(clickRate);
        sniperLaser.enabled = true;
        ableToClick = true;
    }
    private void GunDirection()
    {
        //Follow the direction based on the global scale
        direction = transform.right;
        if (firePoint.transform.lossyScale.x < 0)
        {
            direction = -direction;
        }
    }

    private void SniperShot()
    {
        //Uses distance from regular ray to cast distance
        LayerMask hitLayers = LayerMask.GetMask("Enemy");
        RaycastHit2D[] allHits = Physics2D.RaycastAll(firePoint.transform.position, direction, Mathf.Infinity, hitLayers);
        List<Health> hitEnemies = new List<Health>();
        foreach (var objectHit in allHits)
        {
            Health health = objectHit.collider.GetComponent<Health>();
            if (health == null)
            {
                continue;
            }
            if (hitEnemies.Contains(health)) 
            {
                continue;
            }
            //Declares enemy as already pierced
            //Also adds to the list
            hitEnemies.Add(health); 

            if (hitEnemies.Count > pierceAmount)
            {
                break;
            }
            health.enemyHit(direction, 200, 60);
            Instantiate(enemyHitParticle, objectHit.point, objectHit.transform.rotation);
            //health.currentHealth -= 60;
            //Instantiate(enemyHitParticle, objectHit.point, objectHit.transform.rotation);
        }
    }

    private void Laser()
    {
        //Sets when to stop Ray
        LayerMask includeLayers = LayerMask.GetMask("groundLayer", "wallLayer", "Enemy");
        hit = Physics2D.Raycast(firePoint.transform.position, direction, Mathf.Infinity, includeLayers);

        //Setting laser from sniper
        sniperLaser.SetPosition(0, firePoint.transform.position);
        if (hit.collider != null)
        {
            sniperLaser.SetPosition(1, hit.point);
        }
        else
        {
            sniperLaser.SetPosition(1, firePoint.transform.position + (Vector3)direction * 50f);
        }
    }
}
