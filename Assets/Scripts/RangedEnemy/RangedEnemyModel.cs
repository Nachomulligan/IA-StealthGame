using UnityEngine;

public class RangedEnemyModel : PlayerModel, IDamageable
{
    public float attackRange;
    [SerializeField] public float pursuitRange;
    public LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab; // Prefab de la bala
    [SerializeField] private Transform firePoint; // Punto desde donde se dispara la bala
    [SerializeField] private float bulletSpeed = 10f; // Velocidad de la bala
    [SerializeField] private float bulletLifetime = 3f; // Tiempo de vida de la bala
    [SerializeField] private int bulletDamage = 10; // Da?o que hace la bala

    private Transform playerTransform; // Referencia al jugador
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
        base.Awake();

        // Buscar al jugador
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public override void Attack()
    {
        if (playerTransform != null && bulletPrefab != null && firePoint != null)
        {
            // Mirar hacia el jugador
            _look.LookDir((playerTransform.position - transform.position).normalized);

            // Instanciar la bala
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
