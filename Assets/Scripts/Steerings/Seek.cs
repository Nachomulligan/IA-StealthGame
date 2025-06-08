using UnityEngine;

public class Seek : ISteering
{
    Transform _self;
    Transform _target;

    public Seek(Transform self)
    {
        _self = self;
    }

    public Seek(Transform self, Transform target)
    {
        _self = self;
        _target = target;
    }

    public virtual Vector3 GetDir()
    {
        // Verificar si tenemos un target v?lido
        if (_target == null || _target.gameObject == null)
        {
            return Vector3.zero;
        }

        // a-->b
        // b-a  
        // a: self
        // b: target
        return (_target.position - _self.position).normalized;
    }

    public Transform Target
    {
        set { _target = value; }
        get { return _target; }
    }
}