using UnityEngine;

public class GoonStateGoZone<T> : State<T>
{
    private FlockingManager _flockingManager;
    private GoonEnemyModel _goon;
    private Transform _zone;
    private Seek _seek;

    public GoonStateGoZone(GoonEnemyModel goon, Transform zone, FlockingManager flockingManager)
    {
        _flockingManager = flockingManager;
        _goon = goon;
        _zone = zone;
        _seek = new Seek(goon.transform);
    }

    public override void Enter()
    {
        Debug.Log("go zone");

        base.Enter();
        _flockingManager.SetFlockingActive(FlockingType.Leader, false);
        _flockingManager.SetFlockingActive(FlockingType.Predator, false);
        _seek.Target = _zone;
    }

    public override void Execute()
    {
        var seekDir = _seek.GetDir();
        var flockingDir = _flockingManager.GetDir();

        var steering = (seekDir * 2f + flockingDir).normalized;
        steering.y = 0;

        _goon.Move(steering);
    }

    public override void Exit()
    {
        base.Exit();

        _flockingManager.SetFlockingMultiplier(FlockingType.Avoidance, 1f);
    }
}