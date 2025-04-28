using UnityEngine;

public class RangedEnemysIdle<T> : RangedEnemysBase<T>
{
    private float _currentAngle;

    public override void Enter()
    {
        base.Enter();
        _move.Move(Vector3.zero);
        _currentAngle = 0f; 
    }

    public override void Execute()
    {
        base.Execute();

        float radians = _currentAngle * Mathf.Deg2Rad;

        _currentAngle += 30f * Time.deltaTime; 

        Vector3 dir = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));

        _look.LookDir(dir.normalized);
    }


}