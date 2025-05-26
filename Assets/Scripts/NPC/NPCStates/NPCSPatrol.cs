using UnityEngine;

public class NPCSPatrol<T> : NPCSBase<T>
{
    private ISteering _steering;
    private float _patrolTimer;
    private float _patrolDuration;
    private bool _isTired;

    public NPCSPatrol(ISteering steering, float patrolDuration = 5f)
    {
        _steering = steering;
        _patrolDuration = patrolDuration;
    }

    public override void Enter()
    {
        base.Enter();
        _patrolTimer = 0f;
        _isTired = false;
    }

    public override void Execute()
    {
        base.Execute();
        var dir = _steering.GetDir();
        _move.Move(dir);
        _look.LookDir(dir);

        _patrolTimer += Time.deltaTime;

        if (_patrolTimer >= _patrolDuration)
        {
            _isTired = true;
        }
    }

    // Getter for the FSM to check if patrol time is complete
    public bool IsTired => _isTired;

    public override void Exit()
    {
        base.Exit();
        _patrolTimer = 0f;
        _isTired = false;
    }
}