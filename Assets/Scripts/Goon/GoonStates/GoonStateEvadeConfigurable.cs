using UnityEngine;

public class GoonStateEvadeConfigurable<T> : State<T>
{
    private FlockingManager _flockingManager;
    private GoonEnemyModel _goon;
    private Rigidbody _target;
    private FlockingConfiguration _flockingConfig;
    private string _stateName = "Evade";

    private float _evadeTimer;
    private float _evadeDuration;
    private bool _evadeTimeOver;

    public GoonStateEvadeConfigurable(GoonEnemyModel goon, Rigidbody target,
        FlockingManager flockingManager, FlockingConfiguration flockingConfig, float evadeDuration = 3f)
    {
        _flockingManager = flockingManager;
        _goon = goon;
        _target = target;
        _flockingConfig = flockingConfig;
        _evadeDuration = evadeDuration;
    }

    public override void Enter()
    {
        Debug.Log("evade");
        base.Enter();

        _flockingManager.ApplyConfiguration(_flockingConfig, _stateName);

        _evadeTimer = 0f;
        _evadeTimeOver = false;
    }

    public override void Execute()
    {
        _evadeTimer += Time.deltaTime;
        if (_evadeTimer >= _evadeDuration)
        {
            _evadeTimeOver = true;
        }

        var steering = _flockingManager.GetDir();
        steering.y = 0;
        _goon.Move(steering);
    }

    public override void Exit()
    {
        base.Exit();
        _flockingManager.RestorePreviousState();
        _evadeTimer = 0f;
        _evadeTimeOver = false;
    }

    public bool IsEvadeTimeOver => _evadeTimeOver;
}
