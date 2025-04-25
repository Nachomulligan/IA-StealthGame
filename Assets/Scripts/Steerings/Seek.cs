using UnityEngine;

public class Seek : ISteering
{
    Transform _self;
    Transform _target;
    public Seek(Transform self, Transform target)
    {
        _self = self;
        _target = target;
    }

    public virtual Vector3 GetDir()
    {
        //a-->b
        //b-a
        //a: self
        //b: target
        return (_target.position - _self.position).normalized;
    }
}
