using UnityEngine;

public class Return : ISteering
{
    Transform _self;
    Vector3 _origin;

    public Return(Transform self, Vector3 origin)
    {
        _self = self;
        _origin = origin;
    }

    public virtual Vector3 GetDir()
    {
        return (_origin - _self.position).normalized;
    }
}
