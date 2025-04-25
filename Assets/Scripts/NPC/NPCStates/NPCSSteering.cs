using UnityEngine;

public class NPCSSteering<T> : NPCSBase<T>
{
    ISteering _steering;
    public NPCSSteering(ISteering steering)
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
