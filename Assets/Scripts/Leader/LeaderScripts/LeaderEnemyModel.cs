using System.Collections.Generic;
using UnityEngine;


public class LeaderEnemyModel : BaseEnemyModel
{
    public enum AttackType
    {
        Melee,
        Ranged,
        Explosion
    }
    [Header("Attack Prefabs and Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 3f;
    [SerializeField] private int bulletDamage = 10;

    [Header("Explosion Attack")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float explosionRadius = 8f;

    [Header("Dynamic Roulette Settings")]
    [SerializeField] private float baseWeight = 1f;
    [SerializeField] private float killMultiplier = 0.3f; 
    [SerializeField] private float maxWeightBonus = 5f; 

    private PlayerModel _player;
    private CounterManager _counterManager;
    private RouletteWheelSystem _rouletteSystem;

    private Dictionary<string, AttackType> weaponToAttackType = new Dictionary<string, AttackType>
    {
        { "Weapon 1", AttackType.Melee },    // Arma melee
        { "Weapon 2", AttackType.Ranged },   // Arma ranged (bullets)
        { "Weapon 3", AttackType.Explosion } // Traps
    };
    protected override void Awake()
    {
        base.Awake();
    }
    public void Start()
    {
        _player = ServiceLocator.Instance.GetService<PlayerModel>();
        _counterManager = ServiceLocator.Instance.GetService<CounterManager>();
        _rouletteSystem = RouletteWheelSystem.Instance;
    }
    public override void Attack()
    {
        AttackType selectedAttack = DecideAttackTypeWithRoulette();
        ExecuteAttack(selectedAttack);
    }
    private AttackType DecideAttackTypeWithRoulette()
    {
        Dictionary<AttackType, float> attackWeights = CalculateAttackWeights();
        AttackType selectedAttack = _rouletteSystem.Roulette(attackWeights);
        LogRouletteInfo(attackWeights, selectedAttack);

        return selectedAttack;
    }
    private Dictionary<AttackType, float> CalculateAttackWeights()
    {
        Dictionary<AttackType, float> weights = new Dictionary<AttackType, float>();

        weights[AttackType.Melee] = baseWeight;
        weights[AttackType.Ranged] = baseWeight;
        weights[AttackType.Explosion] = baseWeight;

        foreach (var weaponMapping in weaponToAttackType)
        {
            string weaponName = weaponMapping.Key;
            AttackType attackType = weaponMapping.Value;

            int kills = _counterManager.GetKillsForWeapon(weaponName);
            float bonus = Mathf.Min(kills * killMultiplier, maxWeightBonus);

            weights[attackType] += bonus;
        }

        return weights;
    }
    private void ExecuteAttack(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.Melee:
                ExecuteMeleeAttack();
                break;
            case AttackType.Ranged:
                ExecuteRangedAttack();
                break;
            case AttackType.Explosion:
                ExecuteExplosionAttack();
                break;
        }
    }
    private void ExecuteMeleeAttack()
    {
        Debug.Log("Leader Enemy: MELEE TYPE ATTACK");

        var colls = Physics.OverlapSphere(Position, attackRange, enemyMask);
        for (int i = 0; i < colls.Length; i++)
        {
            PlayerController playerController = colls[i].GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.KillPlayer();
            }
        }
    }
    private void ExecuteRangedAttack()
    {
        Debug.Log("Leader Enemy: RANGED TYPE ATTACK");

        if (_player._playerTransform != null && bulletPrefab != null && firePoint != null)
        {
            _look.LookDir((_player._playerTransform.position - transform.position).normalized);
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletObj.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.Initialize(_player._playerTransform, bulletSpeed, bulletLifetime, bulletDamage);
            }
        }
    }
    private void ExecuteExplosionAttack()
    {
        StartCoroutine(ExplosionAttackCoroutine());
    }

    private System.Collections.IEnumerator ExplosionAttackCoroutine()
    {
        Debug.Log("Leader Enemy: RANGED TYPE EXPLOSION");

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(1f);

        var colls = Physics.OverlapSphere(transform.position, explosionRadius, enemyMask);
        for (int i = 0; i < colls.Length; i++)
        {
            PlayerController playerController = colls[i].GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.KillPlayer();
            }
        }
    }
    private void LogRouletteInfo(Dictionary<AttackType, float> weights, AttackType selected)
    {
        float totalWeight = 0f;
        foreach (var weight in weights.Values)
        {
            totalWeight += weight;
        }

        Debug.Log("LEADER ENEMY ROULETTE ");
        foreach (var kvp in weights)
        {
            float percentage = (kvp.Value / totalWeight) * 100f;
            string weaponName = GetWeaponNameForAttackType(kvp.Key);
            int kills = _counterManager.GetKillsForWeapon(weaponName);
        }
    }

    private string GetWeaponNameForAttackType(AttackType attackType)
    {
        foreach (var kvp in weaponToAttackType)
        {
            if (kvp.Value == attackType)
                return kvp.Key;
        }
        return "Weapon Unknown";
    }
    public void SetRouletteSettings(float newBaseWeight, float newKillMultiplier, float newMaxBonus)
    {
        baseWeight = newBaseWeight;
        killMultiplier = newKillMultiplier;
        maxWeightBonus = newMaxBonus;
    }
    public Dictionary<AttackType, float> GetCurrentAttackProbabilities()
    {
        Dictionary<AttackType, float> weights = CalculateAttackWeights();
        Dictionary<AttackType, float> probabilities = new Dictionary<AttackType, float>();

        float totalWeight = 0f;
        foreach (var weight in weights.Values)
        {
            totalWeight += weight;
        }

        foreach (var kvp in weights)
        {
            probabilities[kvp.Key] = (kvp.Value / totalWeight) * 100f;
        }
        return probabilities;
    }
}