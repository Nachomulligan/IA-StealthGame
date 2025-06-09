using UnityEngine;

public class RangedEnemyModel : BaseEnemyModel
{
    [Header("Ranged Attack")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 3f;
    [SerializeField] private int bulletDamage = 10;

    [SerializeField] private PlayerModel _player;
    protected override void Awake()
    {
        base.Awake();
        _player = ServiceLocator.Instance.GetService<PlayerModel>();
    }
    public override void Attack()
    {
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
}


