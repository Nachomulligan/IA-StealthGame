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

    // Sistema de snapshots para guardar estados previos
    private Dictionary<FlockingType, FlockingState> _previousStates;
    private bool _hasSnapshot = false;

    private void Awake()
    {
        _behaviours = GetComponents<IFlocking>();
        _self = GetComponent<IBoid>();
        _colls = new Collider[maxBoids];
        _boids = new List<IBoid>();
        _previousStates = new Dictionary<FlockingType, FlockingState>();

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
            flocking.SetActive(isActive);
    }

    public void SetFlockingMultiplier(FlockingType type, float multiplier)
    {
        var flocking = GetFlocking(type);
        if (flocking != null)
            flocking.SetMultiplier(multiplier);
    }

    public float GetFlockingMultiplier(FlockingType type)
    {
        var flocking = GetFlocking(type);
        if (flocking != null)
            return flocking.GetMultiplier();
        return 0f;
    }
    public void SaveCurrentState()
    {
        _previousStates.Clear();

        foreach (var kvp in _flockingDict)
        {
            var flockingType = kvp.Key;
            var flocking = kvp.Value;

            var state = new FlockingState(flocking.IsActive, flocking.GetMultiplier());
            _previousStates[flockingType] = state;
        }

        _hasSnapshot = true;

        //Stack
        Debug.Log("FlockingManager: Estado guardado correctamente");
    }

    public void RestorePreviousState()
    {
        if (!_hasSnapshot)
        {
            Debug.LogWarning("FlockingManager: No hay estado previo guardado para restaurar");
            return;
        }

        foreach (var kvp in _previousStates)
        {
            var flockingType = kvp.Key;
            var savedState = kvp.Value;

            var flocking = GetFlocking(flockingType);
            if (flocking != null)
            {
                flocking.SetActive(savedState.isActive);
                flocking.SetMultiplier(savedState.multiplier);
            }
        }

        Debug.Log("FlockingManager: Estado previo restaurado correctamente");
    }

    public bool HasSnapshot => _hasSnapshot;

    public void ClearSnapshot()
    {
        _previousStates.Clear();
        _hasSnapshot = false;
    }

    public void SaveAndSetFlocking(FlockingType type, bool isActive, float multiplier)
    {
        SaveCurrentState();
        SetFlockingActive(type, isActive);
        SetFlockingMultiplier(type, multiplier);
    }

    public void SaveAndSetMultipleFlockings(params (FlockingType type, bool isActive, float multiplier)[] settings)
    {
        SaveCurrentState();

        foreach (var setting in settings)
        {
            SetFlockingActive(setting.type, setting.isActive);
            SetFlockingMultiplier(setting.type, setting.multiplier);
        }
    }

    public IBoid SetSelf { set => _self = value; }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}