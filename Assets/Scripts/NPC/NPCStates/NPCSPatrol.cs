using UnityEngine;

public class NPCSPatrol<T> : NPCSBase<T>
{
    private ISteering _steering;
    public NPCSPatrol(ISteering steering)
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