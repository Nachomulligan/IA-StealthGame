using UnityEngine;

public class NPCModel : PlayerModel, IDamageable
{
    public float attackRange;
   [SerializeField] public float pursuitRange;
    public LayerMask enemyMask;
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
    }
    public override void Attack()
    {
        var colls = Physics.OverlapSphere(Position, attackRange, enemyMask);
        for (int i = 0; i < colls.Length; i++)
        {
            PlayerController playerController = colls[i].GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Die();
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
        Debug.Log("NPC " + name + " ha muerto.");
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

