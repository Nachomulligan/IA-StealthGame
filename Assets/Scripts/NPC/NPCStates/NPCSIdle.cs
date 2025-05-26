using UnityEngine;

public class NPCSIdle<T> : NPCSBase<T>
{
    private float _restTimer;
    private float _restDuration;
    private bool _isRested;

    public NPCSIdle(float restDuration = 5f)
    {
        _restDuration = restDuration;
    }

    public override void Enter()
    {
        base.Enter();
        _move.Move(Vector3.zero);
        _restTimer = 0f;
        _isRested = false;
    }

    public override void Execute()
    {
        base.Execute();
        _restTimer += Time.deltaTime;

        if (_restTimer >= _restDuration)
        {
            _isRested = true;
        }
    }

    // Getter for the FSM to check if rest time is complete
    public bool IsRested => _isRested;

    public override void Exit()
    {
        base.Exit();
        _restTimer = 0f;
        _isRested = false;
    }
}