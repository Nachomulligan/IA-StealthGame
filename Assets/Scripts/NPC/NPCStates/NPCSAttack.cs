using UnityEngine;

public class NPCSAttack<T> : NPCSBase<T>
{
    public override void Enter()
    {
        base.Enter();
        _attack.Attack();
    }
}
