using System.Collections.Generic;
using UnityEngine;

public class LeaderEnemyModel : NPCModel
{
    [SerializeField] private bool useRangedAttack = false;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 3f;
    [SerializeField] private int bulletDamage = 10;
    private PlayerModel _player;

    private void OnEnable()
    {
        EventManager.OnNPCDeath += HandleNPCDeath;
    }

    private void OnDisable()
    {
        EventManager.OnNPCDeath -= HandleNPCDeath;
    }

    private void HandleNPCDeath(IDamageable damageable)
    {
        if ((object)damageable == this)
        {
            Die();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _player = ServiceLocator.Instance.GetService<PlayerModel>();
    }

    public override void Attack()
    {
        if (useRangedAttack)
        {
            //ranged
            if (_player._playerTransform != null && bulletPrefab != null && firePoint != null)
            {
                _look.LookDir((_player._playerTransform.position - transform.position).normalized);
                GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Bullet bullet = bulletObj.GetComponent<Bullet>();

                if (bullet != null)
                {
                    bullet.Initialize(_player._playerTransform, bulletSpeed, bulletLifetime, bulletDamage);
                }
            }
        }
        else
        {
            //melee
            var colls = Physics.OverlapSphere(Position, attackRange, enemyMask);
            for (int i = 0; i < colls.Length; i++)
            {
                PlayerController playerController = colls[i].GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.KillPlayer();
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pursuitRange);
    }
}
