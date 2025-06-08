using System.Collections.Generic;
using UnityEngine;

public class LeaderEnemyModel : BaseEnemyModel
{
    [Header("Leader Attack Settings")]
    [SerializeField] private bool useRangedAttack = false;

    [Header("Attack Probabilities")]
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
        int playerKills = _player.GetKillCount(); // <-- Necesitas que PlayerModel tenga este método

        // Actualiza la probabilidad según las kills
        UpdateAttackProbability(playerKills);

        // Ruleta: decide qué ataque usar
        float roll = Random.Range(0f, 1f);
        useRangedAttack = roll < rangedAttackProbability;
    }

    private void UpdateAttackProbability(int playerKills)
    {
        // Ejemplo de ajuste de probabilidades
        if (playerKills < 5)
        {
            rangedAttackProbability = 0.2f; // 20% ranged
        }
        else if (playerKills < 10)
        {
            rangedAttackProbability = 0.5f; // 50% ranged
        }
        else
        {
            rangedAttackProbability = 0.8f; // 80% ranged
        }
    }

    public void SetAttackType(bool useRanged)
    {
        useRangedAttack = useRanged;
    }

    public bool IsUsingRangedAttack => useRangedAttack;
}