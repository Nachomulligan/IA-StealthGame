using UnityEngine;

public abstract class BaseEnemyModel : BaseEntityModel, IDamageable
{
    [Header("Enemy Stats")]
    [SerializeField] public float attackRange;
    [SerializeField] protected float pursuitRange;
    [SerializeField] protected LayerMask enemyMask;

    public float PursuitRange => pursuitRange;

    protected ObstacleAvoidance _obs;
    protected ILook _look;

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
        _obs = GetComponent<ObstacleAvoidance>();
        _look = GetComponent<ILook>();
    }

    public override void Move(Vector3 dir)
    {
        dir = _obs.GetDir(dir);
        _look.LookDir(dir);
        base.Move(dir);
    }

    public override void Die()
    {
        var _gm = ServiceLocator.Instance.GetService<gameManager>();
        Debug.Log("Enemy " + name + " ha muerto.");
        _gm._enemiesDone += 1;
        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pursuitRange);
    }
}
