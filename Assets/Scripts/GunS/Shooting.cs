using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SearchService;

public class Shooting : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;

    [SerializeField] Animator anim;

    [SerializeField] float fireRate = 0.1f;
    private float nextFire;

    void Update()
    {
        Shoot();
    }

    private void Shoot()
    {
        //checks if im pressing Left Click and Time.time has passed longer than nextFire
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            //By creating it equaling time.time + fireRate we a shot every fireRate per Second
            nextFire = Time.time + fireRate;
            anim.SetBool("isShooting", true);
            Instantiate(bulletPrefab, firePoint.transform.position, firePoint.rotation);
        }
        else
        {
            anim.SetBool("isShooting", false);
        }
    }

}
