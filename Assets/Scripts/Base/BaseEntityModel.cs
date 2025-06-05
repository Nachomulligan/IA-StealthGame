using System;
using UnityEngine;

public abstract class BaseEntityModel : MonoBehaviour, IMove, IAttack
{
    [Header("Base Stats")]
    public float speed;

    protected bool _isDead = false;
    protected Rigidbody _rb;
    protected Action _onAttack = delegate { };

    public Action OnAttack { get => _onAttack; set => _onAttack = value; }
    public Vector3 Position => transform.position;
    public bool IsDead => _isDead;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public virtual void Move(Vector3 dir)
    {
        dir *= speed;
        dir.y = _rb.linearVelocity.y;
        _rb.linearVelocity = dir;
    }

    public abstract void Attack();
    public abstract void Die();
}
