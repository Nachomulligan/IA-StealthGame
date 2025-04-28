using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerModel : MonoBehaviour, IMove, IAttack
{
    public float speed;
    private Rigidbody _rb;
    private Action _onAttack = delegate { };
    public float playerAttackRange = 5f;
    public LayerMask enemyLayer;

    private bool _canAttack = false;

    public Action OnAttack { get => _onAttack; set => _onAttack = value; }
    public Vector3 Position => transform.position;



    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        ServiceLocator.Instance.Register<PlayerModel>(this);
    }

    public void EnableAttack()
    {
        _canAttack = true; 
    }

    public virtual void Attack()
    {
        if (!_canAttack)
        {
            UnityEngine.Debug.Log("No puedes atacar, no tienes un arma.");
            return;
        }

        UnityEngine.Debug.Log("¡Player realizó un ataque!");

        var colls = Physics.OverlapSphere(transform.position, playerAttackRange, enemyLayer);
        foreach (var col in colls)
        {
            var damageable = col.GetComponent<IDamageable>();
            if (damageable != null)
            {
                EventManager.InvokeNPCDeath(damageable);
            }
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