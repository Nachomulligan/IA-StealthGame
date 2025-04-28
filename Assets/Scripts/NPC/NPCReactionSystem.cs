using UnityEngine;
public class NPCReactionSystem : MonoBehaviour
{
    [Header("Weapon Reaction Settings")]
    [Tooltip("player has weapon")]
    public bool playerHasWeapon = false;

    [Range(0, 100)]
    [Tooltip("Weapon chase prob")]
    public float chaseWeightWithWeapon = 60f;

    [Range(0, 100)]
    [Tooltip("weapon evade prob")]
    public float evadeWeightWithWeapon = 40f;

    [Header("Reaction State")]
    [Tooltip("has seen player for the first time")]
    public bool hasSeenPlayerFirstTime = false;

    [Tooltip("desicion taken, should I evade?")]
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
    
    //Sees if enemy is out, get out of range
    public bool DecideIfShouldEvade()
    {
        // If descicion took, keep it
        if (hasSeenPlayerFirstTime)
        {
            return shouldEvade;
        }
        // Player seen for the first time, remember
        hasSeenPlayerFirstTime = true;
        if (playerHasWeapon)
        {
            // decision wheel
            shouldEvade = RouletteWheelSystem.Instance.ShouldEvadeTarget(
                chaseWeightWithWeapon,
                evadeWeightWithWeapon
            );

            Debug.Log($"Player seen with weapon for the firs time. Decisión: {(shouldEvade ? "EVADE" : "chase")}");
        }
        else
        {
            // has no gun, always chase
            shouldEvade = false;
            Debug.Log("Player with no weapon, chase");
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
