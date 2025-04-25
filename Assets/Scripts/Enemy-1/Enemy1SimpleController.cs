using UnityEngine;

public class Enemy1SimpleController : PlayerModel
{
    [SerializeField] private Transform _waypointGroup;
    ILook _look;
    public LayerMask enemyMask;
    ObstacleAvoidance _obstacleAvoidance;
    FSM<StateEnum> _fsm;
    private void Start()
    {
        var patrolState = new E1PatrolState<Enemy1SimpleController>(_waypointGroup);
    }
    protected override void Awake()
    {
        _obstacleAvoidance = GetComponent<ObstacleAvoidance>();
        _look = GetComponent<ILook>();
        base.Awake();
    }
    public override void Move(Vector3 dir)
    {
        dir = _obstacleAvoidance.GetDir(dir);
        _look.LookDir(dir);
        base.Move(dir);
    }
}
