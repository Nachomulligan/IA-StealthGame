using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerModel : MonoBehaviour, IMove, IAttack
{
    [Header("Player Stats")]
    public float speed;
    public float playerAttackRange = 5f;
    public LayerMask enemyLayer;

    [Header("Weapon System")]
    [SerializeField] private Transform weaponPoint;
    [SerializeField] private GameObject currentWeapon;

    private bool _isArmed = false;
    private bool _isDead = false;
    private bool _canAttack = false;


    private Rigidbody _rb;
    private Action _onAttack = delegate { };

    public static event Action<bool> OnPlayerArmedChanged;
    public static event Action OnPlayerDied;

    public Action OnAttack { get => _onAttack; set => _onAttack = value; }
    public Vector3 Position => transform.position;
    public bool IsArmed => _isArmed;
    public bool IsDead => _isDead;
    public bool CanAttack => _canAttack;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        ServiceLocator.Instance.Register<PlayerModel>(this);
    }

    public void EquipWeapon(GameObject weapon)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        currentWeapon = Instantiate(weapon, weaponPoint.position, weaponPoint.rotation, weaponPoint);
        _isArmed = (currentWeapon != null);
        _canAttack = _isArmed;

        OnPlayerArmedChanged?.Invoke(_isArmed);

        UnityEngine.Debug.Log($"Weapon equipped: {weapon.name}. Player is now armed: {_isArmed}");
    }

    public virtual void Die()
    {
        if (_isDead) return; // Evitar múltiples muertes

        _isDead = true;
        var _gm = ServiceLocator.Instance.GetService<gameManager>();

        UnityEngine.Debug.Log("The player has died.");

        if (_gm != null)
        {
            _gm._isDead = true;
        }

        OnPlayerDied?.Invoke();
        Destroy(gameObject);
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
