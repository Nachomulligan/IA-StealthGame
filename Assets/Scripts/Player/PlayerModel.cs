using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerModel : MonoBehaviour, IMove, IAttack
{
    public float speed;
    Rigidbody _rb;
    Action _onAttack = delegate { };
    public float playerAttackRange = 5f;
    public LayerMask enemyLayer;

    public Action OnAttack { get => _onAttack; set => _onAttack = value; }
    public Vector3 Position => transform.position;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public virtual void Attack()
    {
        UnityEngine.Debug.Log("¡Player realizó un ataque!");
        var colls = Physics.OverlapSphere(Position, playerAttackRange, enemyLayer);
        for (int i = 0; i < colls.Length; i++)
        {
            GameObject.Destroy(colls[i].gameObject);
        }

        _onAttack();

    }
    public virtual void Move(Vector3 dir)
    {
        dir *= speed;
        dir.y = _rb.linearVelocity.y;
        _rb.linearVelocity = dir;
    }
}
