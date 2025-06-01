using UnityEngine;

public class GoonEnemy : MonoBehaviour, IBoid, ILook, IMove 
{
    public float speed;
    public float speedRot = 5;

    Rigidbody _rb;
    public Vector3 Position => transform.position;
    public Vector3 Forward => transform.forward;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public void Move(Vector3 dir)
    {
        dir *= speed;
        dir.y = _rb.linearVelocity.y;
        _rb.linearVelocity = dir;
    }
    public void LookDir(Vector3 dir)
    {
        if (dir.x == 0 && dir.z == 0) return;
        transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * speedRot);
    }
}
