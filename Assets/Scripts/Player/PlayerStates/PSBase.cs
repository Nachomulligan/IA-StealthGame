using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSBase<T> : State<T>
{
    protected IMove _move;
    protected ILook _look;
    protected IAttack _attack;
    public override void Initialize(params object[] p)
    {
        base.Initialize(p);
        _move = p[0] as IMove;
        _look = p[1] as ILook;
        _attack = p[2] as IAttack;
    }
}
