using UnityEngine;
public class RangedEnemyReactionSystem : MonoBehaviour
{
    [Header("Weapon Reaction Settings")]
    [Tooltip("player has weapon")]
    public bool playerHasWeapon = false;

    [Range(0, 100)]
    [Tooltip("chase prob when player has weapon")]
    public float chaseWeightWithWeapon = 60f;

    [Range(0, 100)]
    [Tooltip("evade prob when player has weapon")]
    public float evadeWeightWithWeapon = 40f;

    [Header("Reaction State")]
    [Tooltip("player seen for the first time")]
    public bool hasSeenPlayerFirstTime = false;

    [Tooltip("desicion taken, evade")]
    public bool shouldEvade = false;
    private void OnEnable()
    {
        PlayerController.OnPlayerArmedChanged += UpdateWeaponStatus;
    }
    private void OnDisable()
    {
        PlayerController.OnPlayerArmedChanged -= UpdateWeaponStatus;
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
        Debug.Log("NPC REACTION RESET");
    }
}
