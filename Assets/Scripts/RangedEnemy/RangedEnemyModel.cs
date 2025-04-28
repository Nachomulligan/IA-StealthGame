using UnityEngine;

public class RangedEnemyModel : PlayerModel, IDamageable
{
    public float attackRange;
    [SerializeField] public float pursuitRange;
    public LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 3f;
    [SerializeField] private int bulletDamage = 10; 
    private gameManager _gm;
    private Transform playerTransform;
    ObstacleAvoidance _obs;
    ILook _look;

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
        _obs = GetComponent<ObstacleAvoidance>();
        _look = GetComponent<ILook>();
        _gm = FindFirstObjectByType<gameManager>();
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

    public override void Move(Vector3 dir)
    {
        dir = _obs.GetDir(dir);
        _look.LookDir(dir);
        base.Move(dir);
    }

    public void Die()
    {
        Debug.Log("Enemigo a distancia " + name + " ha muerto.");
        _gm._enemiesDone += 1;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pursuitRange);
    }
}
