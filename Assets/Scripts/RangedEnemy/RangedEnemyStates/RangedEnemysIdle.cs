using UnityEngine;

public class RangedEnemysIdle<T> : NPCSBase<T>
{
    private float _restTimer;
    private float _restDuration;
    private bool _isRested;
    private float _currentAngle;

    public RangedEnemysIdle(float restDuration = 5f)
    {
        _restDuration = restDuration;
    }

    public override void Enter()
    {
        base.Enter();
        _move.Move(Vector3.zero);
        _restTimer = 0f;
        _isRested = false;
        _currentAngle = 0f;
    }

    public override void Execute()
    {
        base.Execute();

        // Rotación
        float radians = _currentAngle * Mathf.Deg2Rad;
        _currentAngle += 30f * Time.deltaTime;
        Vector3 dir = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));
        _look.LookDir(dir.normalized);

        // Temporizador
        _restTimer += Time.deltaTime;
        if (_restTimer >= _restDuration)
            _isRested = true;
    }

    public bool IsRested => _isRested;

    public override void Exit()
    {
        base.Exit();
        _restTimer = 0f;
        _isRested = false;
    }
}