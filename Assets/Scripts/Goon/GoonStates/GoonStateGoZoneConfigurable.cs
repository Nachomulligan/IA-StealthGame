using System;
using UnityEngine;

public class GoonStateGoZoneConfigurable<T> : State<T>
{
    private FlockingManager _flockingManager;
    private GoonEnemyModel _goon;
    private Transform _zone;
    private FlockingConfiguration _flockingConfig;
    private SeekBehaviour _seekBehaviour;
    private string _stateName = "GoZone";

    [Header("Zone Settings")]
    public float arrivalThreshold = 8f;
    private bool _hasArrivedAtZone = false;

    public GoonStateGoZoneConfigurable(GoonEnemyModel goon, Transform zone,
        FlockingManager flockingManager, FlockingConfiguration flockingConfig, float threshold = 8f)
    {
        _flockingManager = flockingManager;
        _goon = goon;
        _zone = zone;
        _flockingConfig = flockingConfig;
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

        _flockingManager.ApplyConfiguration(_flockingConfig, _stateName);

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
