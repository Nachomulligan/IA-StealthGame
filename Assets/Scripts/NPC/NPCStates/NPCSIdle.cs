using UnityEngine;

public class NPCSIdle<T> : NPCSBase<T>
{
    public override void Enter()
    {
        base.Enter();
        _move.Move(Vector3.zero);
    }
}
