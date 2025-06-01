using System.Collections.Generic;
using UnityEngine;

public class GenericBehaviour : FlockingBaseBehaviour
{
    Vector3 _dir;
    protected override Vector3 GetRealDir(List<IBoid> boids, IBoid self)
    {
        return _dir.normalized * multiplier;
    }
    public Vector3 Dir
    {
        get { return _dir; }
        set { _dir = value; }
    }
}
