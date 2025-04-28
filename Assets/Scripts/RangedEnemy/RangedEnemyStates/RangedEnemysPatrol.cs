using UnityEngine;

public class RangedEnemysPatrol<T> : RangedEnemysBase<T>
{
    private ISteering _steering;
    public RangedEnemysPatrol(ISteering steering)
    {
        _steering = steering;
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Execute()
    {
        base.Execute();
        var dir = _steering.GetDir();
        _move.Move(dir);
        _look.LookDir(dir);
    }
}