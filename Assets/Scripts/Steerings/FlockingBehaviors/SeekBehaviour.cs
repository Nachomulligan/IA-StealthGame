using System.Collections.Generic;
using UnityEngine;

public class SeekBehaviour : FlockingBaseBehaviour
{
    [Header("Seek Settings")]
    public Transform target;

    private void Awake()
    {
        if (_flockingType != FlockingType.Seek)
            _flockingType = FlockingType.Seek;
    }

    protected override Vector3 GetRealDir(List<IBoid> boids, IBoid self)
    {
        if (target == null || target.gameObject == null)
            return Vector3.zero;

        Vector3 direction = (target.position - self.Position).normalized;
        return direction * multiplier;
    }

    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }
}