using UnityEngine;
public abstract class BaseFlockingEnemyModel : BaseEntityModel, IDamageable
{
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
}
