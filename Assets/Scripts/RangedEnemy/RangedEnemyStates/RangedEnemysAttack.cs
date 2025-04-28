using UnityEngine;

public class RangedEnemysAttack<T> : RangedEnemysBase<T>
{
    public override void Enter()
    {
        base.Enter();
        _attack.Attack();
    }
}
