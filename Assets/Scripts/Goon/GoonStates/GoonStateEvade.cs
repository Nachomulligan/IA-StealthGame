using UnityEngine;

public class GoonStateEvade<T> : State<T>
{
    private FlockingManager _flockingManager;
    private GoonEnemyModel _goon;
    private Rigidbody _target;
    private Evade _evade;

    private float _evadeTimer;
    private float _evadeDuration;
    private bool _evadeTimeOver;

    public GoonStateEvade(GoonEnemyModel goon, Rigidbody target, FlockingManager flockingManager, float evadeDuration = 3f)
    {
        _flockingManager = flockingManager;
        _goon = goon;
        _target = target;
        _evadeDuration = evadeDuration;
    }

    public override void Enter()
    {
        Debug.Log("evade");
        base.Enter();

        _evadeTimer = 0f;
        _evadeTimeOver = false;

        _flockingManager.SetFlockingActive(FlockingType.Predator, true);
        _flockingManager.SetFlockingActive(FlockingType.Avoidance, true);

        _flockingManager.SetFlockingActive(FlockingType.Leader, false);

        _flockingManager.SetFlockingMultiplier(FlockingType.Predator, 10f);
        _flockingManager.SetFlockingMultiplier(FlockingType.Avoidance, 2f);
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

        _evadeTimer = 0f;
        _evadeTimeOver = false;

    }

    public bool IsEvadeTimeOver => _evadeTimeOver;
}