using UnityEngine;

public class Evade : Pursuit
{
    public Evade(Transform self, Rigidbody target) : base(self, target)
    {
    }
    public Evade(Transform self, Rigidbody target, float errorRange = 0, float timePrediction = 0) : base(self, target, errorRange, timePrediction)
    {
    }
    public Evade(Transform self, Rigidbody target, float errorRange = 0) : base(self, target, errorRange)
    {
    }
    public override Vector3 GetDir()
    {
        if (_target == null || _target.gameObject == null)
        {
            return Vector3.zero;
        }

        Vector3 point = _target.position + _target.linearVelocity * _timePrediction;

        Vector3 dirToPoint = (point - _self.position).normalized;

        Vector3 dirToTarget = (_target.position - _self.position).normalized;

        if (Vector3.Dot(dirToPoint, -dirToTarget) < 0 + _errorRange)
        {
            return -dirToTarget;
        }
        else
        {
            return -dirToPoint;
        }
    }
}
    