using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerModel : BaseEntityModel
{
    [Header("Player Stats")]
    public float playerAttackRange = 5f;
    public LayerMask enemyLayer;
    [SerializeField] private Transform PlayerTransform;
    public Transform _playerTransform => PlayerTransform;

    [Header("Weapon System")]
    [SerializeField] private Transform weaponPoint;
    public GameObject currentWeapon;
    private CounterManager _counterManager;

    private bool _isArmed = false;
    private bool _canAttack = false;

    public static event Action<bool> OnPlayerArmedChanged;
    public static event Action OnPlayerDied;

    public bool IsArmed => _isArmed;
    public bool CanAttack => _canAttack;

    protected override void Awake()
    {
        base.Awake();
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
    public override void Die()
    {
        if (_isDead) return;

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
    public override void Attack()
    {
        if (!_canAttack)
        {
            UnityEngine.Debug.Log("Cant Attack, No weapon");
            return;
        }
        UnityEngine.Debug.Log("Player Attacked");
        var colls = Physics.OverlapSphere(transform.position, playerAttackRange, enemyLayer);

        foreach (var col in colls)
        {
            var damageable = col.GetComponent<IDamageable>();
            if (damageable != null)
            {
                EventManager.InvokeNPCDeath(damageable);
                _counterManager = ServiceLocator.Instance.GetService<CounterManager>();
                CounterManager.Instance.RegisterKill("Weapon 1");
            }
        }
        _onAttack();
    }
    public int GetKillCount()
    {
        if (_counterManager == null)
        {
            _counterManager = ServiceLocator.Instance.GetService<CounterManager>();
        }

        return _counterManager != null ? _counterManager.GetTotalKills() : 0;
    }
}