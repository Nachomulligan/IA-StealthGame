using System.Collections.Generic;
using UnityEngine;

public class FlockingBaseBehaviour : MonoBehaviour, IFlocking
{
    [SerializeField] private float _multiplier = 1f;
    [SerializeField] private bool _isActive = true;
    [SerializeField] public FlockingType _flockingType;

    public FlockingType FlockingType => _flockingType;

    public Vector3 GetDir(List<IBoid> boids, IBoid self)
    {
        if (_isActive)
            return GetRealDir(boids, self);
        else
            return Vector3.zero;
    }

    protected virtual Vector3 GetRealDir(List<IBoid> boids, IBoid self)
    {
        return Vector3.zero;
    }

    public bool IsActive
    {
        get { return _isActive; }
        set => _isActive = value;
    }

    public void SetMultiplier(float multiplier)
    {
        _multiplier = multiplier;
    }

    public float GetMultiplier()
    {
        return _multiplier;
    }

    public void SetActive(bool active)
    {
        _isActive = active;
    }

    public float multiplier
    {
        get => _multiplier;
        set => _multiplier = value;
    }
}