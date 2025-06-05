using System.Collections.Generic;
using UnityEngine;

public class LeaderEnemyModel : BaseEnemyModel
{
    [Header("Leader Attack Settings")]
    [SerializeField] private bool useRangedAttack = false;

    [Header("Ranged Attack")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 3f;
    [SerializeField] private int bulletDamage = 10;

    private PlayerModel _player;

    protected override void Awake()
    {
        base.Awake();
    }
    public void Start()
    {
        _player = ServiceLocator.Instance.GetService<PlayerModel>();
    }
    public override void Attack()
    {
        if (useRangedAttack)
        {
            // Ataque a distancia
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
            // Ataque cuerpo a cuerpo
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
    public void SetAttackType(bool useRanged)
    {
        useRangedAttack = useRanged;
    }
    public bool IsUsingRangedAttack => useRangedAttack;
}
