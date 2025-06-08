using System.Collections.Generic;
using UnityEngine;

public class LeaderEnemyModel : BaseEnemyModel
{
    [Header("Leader Attack Settings")]
    [SerializeField] private bool useRangedAttack = false;

    [Header("Ranged Attack Probability")]
    [Range(0f, 1f)]
    [SerializeField] private float rangedAttackProbability = 0.3f; // Inicialmente 30%

    [Header("Ranged Attack")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifetime = 3f;
    [SerializeField] private int bulletDamage = 10;

    private PlayerModel _player;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        _player = ServiceLocator.Instance.GetService<PlayerModel>();
    }

    public override void Attack()
    {
        DecideAttackType();

        if (useRangedAttack)
        {
            // Ataque a distancia
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
        else
        {
            // Ataque cuerpo a cuerpo
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
    }

    private void DecideAttackType()
    {
        // Obtén el número de kills del player
        int playerKills = _player.GetKillCount();

        // Actualiza la probabilidad de ranged (y melee implícitamente)
        UpdateAttackProbability(playerKills);

        // Ruleta: decide qué ataque usar
        float roll = Random.Range(0f, 1f);
        useRangedAttack = roll < rangedAttackProbability;
    }

    private void UpdateAttackProbability(int playerKills)
    {
        // Base probability
        float baseProbability = 0.2f; // 20% inicial ranged

        // Multiplicador: +5% por kill
        float multiplierPerKill = 0.05f; // +5% por kill

        // Calcula la nueva probabilidad de ranged
        rangedAttackProbability = baseProbability + (playerKills * multiplierPerKill);

        // Clampea entre 0% y 100%
        rangedAttackProbability = Mathf.Clamp01(rangedAttackProbability);

        // No necesitas variable extra: meleeProbability = 1 - rangedAttackProbability
    }

    public void SetAttackType(bool useRanged)
    {
        useRangedAttack = useRanged;
    }

    public bool IsUsingRangedAttack => useRangedAttack;
}