using UnityEngine;

public class Pursuit : ISteering
{
    Transform _self;
    Rigidbody _target;
    float _timePrediction;
    float _errorRange = 0.1f;

    public Pursuit(Transform self, Rigidbody target, float errorRange = 0, float timePrediction = 0)
    {
        _self = self;
        _target = target;
        _timePrediction = timePrediction;
        _errorRange = errorRange;
    }

    public Pursuit(Transform self, float errorRange = 0, float timePrediction = 0)
    {
        _self = self;
        _timePrediction = timePrediction;
        _errorRange = errorRange;
    }

    public Pursuit(Transform self, Rigidbody target, float errorRange = 0)
    {
        _self = self;
        _target = target;
        _errorRange = errorRange;
    }

    public Pursuit(Transform self, Rigidbody target)
    {
        _self = self;
        _target = target;
    }

    public Pursuit(Transform self)
    {
        _self = self;
    }

    public virtual Vector3 GetDir()
    {
        
        if (_target == null || _target.gameObject == null)
        {
            return Vector3.zero;
        }

        Vector3 point = _target.position + _target.linearVelocity * _timePrediction;
        Vector3 dirToPoint = (point - _self.position).normalized;
        Vector3 dirToTarget = (_target.position - _self.position).normalized;

        if (Vector3.Dot(dirToPoint, dirToTarget) < 0 + _errorRange)
        {
            return dirToTarget;
        }
        else
        {
            return dirToPoint;
        }
    }

    public float TimePrediction
    {
        get { return _timePrediction; }
        set { _timePrediction = value; }
    }

    public Rigidbody Target
    {
        set => _target = value;
        get => _target;
    }
}
