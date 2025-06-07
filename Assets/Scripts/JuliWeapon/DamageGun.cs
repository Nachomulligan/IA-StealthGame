using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGun
{
    public float Damage;
    public float BulletRange;
    private Transform PlayerCamera;

    private void Start()
    {
        PlayerCamera = Camera.main.transform;
    }

    public void Shoot()
    {
        Ray gunRay = Ray(PlayerCamera.position, PlayerCamera.forward);
        if (Physics.Raycast(gunRay, out RaycastHit hitInfo, BulletRange))
        {
           
            if (hitInfo.collider.gameObject.TryGetComponent(out BaseEntityModel enemy)
            {
                enemy.Health -= Damage;
            }
        }
    }

}
