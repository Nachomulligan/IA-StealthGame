using UnityEngine;

public class RangedEnemyModel : NPCModel
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 3f;
    [SerializeField] private int bulletDamage = 10; 
    private Transform playerTransform;

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

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public override void Attack()
    {
        if (playerTransform != null && bulletPrefab != null && firePoint != null)
        {
            _look.LookDir((playerTransform.position - transform.position).normalized);
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletObj.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.Initialize(playerTransform, bulletSpeed, bulletLifetime, bulletDamage);
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
