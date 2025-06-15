using System;
using UnityEngine;

public class GoonStateGoZone<T> : State<T>
{
    private FlockingManager _flockingManager;
    private GoonEnemyModel _goon;
    private Transform _zone;
    private SeekBehaviour _seekBehaviour;

    [Header("Zone Settings")]
    public float arrivalThreshold = 8f;
    private bool _hasArrivedAtZone = false;

    public GoonStateGoZone(GoonEnemyModel goon, Transform zone, FlockingManager flockingManager, float threshold = 8f)
    {
        _flockingManager = flockingManager;
        _goon = goon;
        _zone = zone;
        arrivalThreshold = threshold;

        _seekBehaviour = goon.GetComponent<SeekBehaviour>();
        if (_seekBehaviour == null)
        {
            _seekBehaviour = goon.gameObject.AddComponent<SeekBehaviour>();
        }
    }

    public override void Enter()
    {
        Debug.Log("Entering GoZone state");
        base.Enter();

        _flockingManager.SaveCurrentState();

        _flockingManager.SetFlockingActive(FlockingType.Leader, false);
        _flockingManager.SetFlockingActive(FlockingType.Predator, false);
        _flockingManager.SetFlockingActive(FlockingType.Alignment, true);
        _flockingManager.SetFlockingActive(FlockingType.Cohesion, true);
        _flockingManager.SetFlockingActive(FlockingType.Seek, true);

        _seekBehaviour.Target = _zone;
        _hasArrivedAtZone = false;
    }

    public override void Execute()
    {
        if (_zone == null)
        {
            Debug.LogWarning("Zone target is null in GoZone state");
            return;
        }

        float distanceToZone = Vector3.Distance(_goon.transform.position, _zone.position);

        if (distanceToZone <= arrivalThreshold)
        {
            _hasArrivedAtZone = true;
            Debug.Log($"Arrived at zone! Distance: {distanceToZone:F2}");
        }

        Vector3 flockingDir = _flockingManager.GetDir();
        flockingDir.y = 0;
        _goon.Move(flockingDir);
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exiting GoZone state");

        _flockingManager.RestorePreviousState();

        _hasArrivedAtZone = false;
    }

    public bool HasArrivedAtZone => _hasArrivedAtZone;
}