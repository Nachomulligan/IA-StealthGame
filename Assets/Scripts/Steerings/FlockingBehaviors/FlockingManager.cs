using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour, ISteering
{
    [Min(1)]
    public int maxBoids = 10;
    [Min(1)]
    public int radius = 10;
    public LayerMask boidMask;

    IFlocking[] _behaviours;
    Dictionary<FlockingType, IFlocking> _flockingDict;
    IBoid _self;
    Collider[] _colls;
    List<IBoid> _boids;

    private void Awake()
    {
        _behaviours = GetComponents<IFlocking>();
        _self = GetComponent<IBoid>();
        _colls = new Collider[maxBoids];
        _boids = new List<IBoid>();

        InitializeFlockingDictionary();
    }

    private void InitializeFlockingDictionary()
    {
        _flockingDict = new Dictionary<FlockingType, IFlocking>();

        for (int i = 0; i < _behaviours.Length; i++)
        {
            var behaviour = _behaviours[i];
            _flockingDict[behaviour.FlockingType] = behaviour;
        }
    }

    public Vector3 GetDir()
    {
        _boids.Clear();
        int count = Physics.OverlapSphereNonAlloc(_self.Position, radius, _colls, boidMask);

        for (int i = 0; i < count; i++)
        {
            var boid = _colls[i].GetComponent<IBoid>();
            if (boid == null || boid == _self) continue;
            _boids.Add(boid);
        }

        Vector3 dir = Vector3.zero;
        for (int i = 0; i < _behaviours.Length; i++)
        {
            var curr = _behaviours[i];
            dir += curr.GetDir(_boids, _self);
        }
        return dir.normalized;
    }

    public IFlocking GetFlocking(FlockingType type)
    {
        if (_flockingDict.ContainsKey(type))
            return _flockingDict[type];
        return null;
    }

    public void SetFlockingActive(FlockingType type, bool isActive)
    {
        var flocking = GetFlocking(type);
        if (flocking != null)
            flocking.IsActive = isActive;
    }

    public void SetFlockingMultiplier(FlockingType type, float multiplier)
    {
        var flocking = GetFlocking(type);
        if (flocking != null && flocking is FlockingBaseBehaviour baseBehaviour)
        {
            baseBehaviour.multiplier = multiplier;
        }
    }

    public float GetFlockingMultiplier(FlockingType type)
    {
        var flocking = GetFlocking(type);
        if (flocking != null && flocking is FlockingBaseBehaviour baseBehaviour)
        {
            return baseBehaviour.multiplier;
        }
        return 0f;
    }

    public IBoid SetSelf { set => _self = value; }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}