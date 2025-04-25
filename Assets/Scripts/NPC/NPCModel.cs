using UnityEngine;

public class NPCModel : PlayerModel
{
    public float attackRange;
    public LayerMask enemyMask;
    ObstacleAvoidance _obs;
    ILook _look;

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
            GameObject.Destroy(colls[i].gameObject);
        }
        base.Attack();
    }
    public override void Move(Vector3 dir)
    {
        dir = _obs.GetDir(dir);
        _look.LookDir(dir);
        base.Move(dir);
    }
}
