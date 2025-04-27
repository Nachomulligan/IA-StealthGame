using UnityEngine;

public class Flee : ISteering
{
    private Transform _self;
    private Transform _target;
    private float _fleeDistance; 

    public Flee(Transform self, Transform target, float fleeDistance = 5f)
    {
        _self = self;
        _target = target;
        _fleeDistance = fleeDistance;
    }

    public Vector3 GetDir()
    {
        Vector3 dir = _self.position - _target.position;

        if (dir.magnitude < _fleeDistance)
        {
            return dir.normalized;
        }
        else
        {
            return Vector3.zero; 
        }
    }
}