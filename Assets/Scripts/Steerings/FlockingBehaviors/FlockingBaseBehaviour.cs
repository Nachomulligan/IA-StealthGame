using System.Collections.Generic;
using UnityEngine;

public class FlockingBaseBehaviour : MonoBehaviour, IFlocking
{
    public float multiplier = 1;
    bool _isActive = true;
    public Vector3 GetDir(List<IBoid> boids, IBoid self)
    {
        if (_isActive)
            return GetRealDir(boids, self);
        else return Vector3.zero;
    }
    protected virtual Vector3 GetRealDir(List<IBoid> boids, IBoid self)
    {
        return Vector3.zero;
    }
    public bool IsActive { get { return _isActive; } set => _isActive = value; }
}
