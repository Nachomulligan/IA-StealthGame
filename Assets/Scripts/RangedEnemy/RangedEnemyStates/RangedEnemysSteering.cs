using UnityEngine;

public class RangedEnemysSteering<T> : RangedEnemysBase<T>
{
    ISteering _steering;
    public RangedEnemysSteering(ISteering steering)
    {
        _steering = steering;
    }
    public override void Execute()
    {
        base.Execute();
        var dir = _steering.GetDir();
        _move.Move(dir.normalized);
    }
    public void ChangeSteering(ISteering newSteering)
    {
        _steering = newSteering;
    }
}
