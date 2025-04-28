using UnityEngine;

public class RangedEnemysChase<T> : RangedEnemysBase<T>
{
    Transform _target;
    public RangedEnemysChase(Transform target)
    {
        _target = target;
    }
    public override void Execute()
    {
        base.Execute();
        //a-->b
        //b-a
        //a= self 
        //b=target

        var dir = _target.transform.position - _move.Position;
        _move.Move(dir.normalized);
        _look.LookDir(dir.normalized);
    }
}

