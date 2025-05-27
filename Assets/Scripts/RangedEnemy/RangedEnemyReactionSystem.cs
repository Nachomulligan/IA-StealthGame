using UnityEngine;
public class RangedEnemyReactionSystem : MonoBehaviour
{
    [Header("Weapon Reaction Settings")]
    [Tooltip("¿El jugador tiene un arma?")]
    public bool playerHasWeapon = false;

    [Range(0, 100)]
    [Tooltip("Porcentaje de probabilidad de perseguir cuando el jugador tiene un arma")]
    public float chaseWeightWithWeapon = 60f;

    [Range(0, 100)]
    [Tooltip("Porcentaje de probabilidad de evadir cuando el jugador tiene un arma")]
    public float evadeWeightWithWeapon = 40f;

    [Header("Reaction State")]
    [Tooltip("¿Ya ha visto al jugador por primera vez?")]
    public bool hasSeenPlayerFirstTime = false;

    [Tooltip("Decisión tomada: ¿Deberú} evadir?")]
    public bool shouldEvade = false;


    private void OnEnable()
    {
        PlayerModel.OnPlayerArmedChanged += UpdateWeaponStatus;
    }

    private void OnDisable()
    {
        PlayerModel.OnPlayerArmedChanged -= UpdateWeaponStatus;
    }

    private void UpdateWeaponStatus(bool isArmed)
    {
        playerHasWeapon = isArmed;
    }

    public bool DecideIfShouldEvade()
    {
        // Desicion taken, keep it
        if (hasSeenPlayerFirstTime)
        {
            return shouldEvade;
        }
        // first time player been spotted, tagging it
        hasSeenPlayerFirstTime = true;

        if (playerHasWeapon)
        {
            // roulette to choose
            shouldEvade = RouletteWheelSystem.Instance.ShouldEvadeTarget(
                chaseWeightWithWeapon,
                evadeWeightWithWeapon
            );

            Debug.Log($"player seen with weapon for the first time {(shouldEvade ? "evade" : "chase")}");
        }
        else
        {
            shouldEvade = false;
            Debug.Log("Player without weapon, chase");
        }
        return shouldEvade;
    }

    public void ResetReaction()
    {
        hasSeenPlayerFirstTime = false;
        shouldEvade = false;
        Debug.Log("Reacción del NPC reiniciada");
    }
}
