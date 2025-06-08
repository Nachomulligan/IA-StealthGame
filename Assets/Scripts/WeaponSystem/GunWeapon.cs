using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GunWeapon : MonoBehaviour
{
    [SerializeField] private Transform shootPos; 
    [SerializeField] private GameObject bulletPrefab; 
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 0.5f;

    private float lastFireTime = 0f;

    public void Fire()
    {
        if (Time.time < lastFireTime + fireRate)
            return;

        lastFireTime = Time.time;

        GameObject bullet = Instantiate(bulletPrefab, shootPos.position, shootPos.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootPos.forward * bulletSpeed;
        }

       UnityEngine. Debug.Log("Fired bullet!");
    }
}