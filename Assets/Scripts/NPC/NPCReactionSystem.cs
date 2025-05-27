using UnityEngine;
public class NPCReactionSystem : MonoBehaviour
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

    [Tooltip("Decisión tomada: ¿Debería evadir?")]
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

    /// <summary>
    /// Determina si el NPC debería evadir al jugador basado en si tiene un arma
    /// </summary>
    /// <returns>True si debería evadir, False si debería perseguir</returns>
    public bool DecideIfShouldEvade()
    {
        // Si ya hemos tomado esta decisión, mantenerla
        if (hasSeenPlayerFirstTime)
            return shouldEvade;

        // Primera vez que vemos al jugador, marcamos que ya lo vimos
        hasSeenPlayerFirstTime = true;

        if (playerHasWeapon)
        {
            // Usar el sistema de ruleta para decidir
            shouldEvade = RouletteWheelSystem.Instance.ShouldEvadeTarget(
                chaseWeightWithWeapon,
                evadeWeightWithWeapon
            );

            Debug.Log($"Primera vez viendo al jugador con arma. Decisión: {(shouldEvade ? "EVADIR" : "PERSEGUIR")}");
        }
        else
        {
            // Si no tiene arma, siempre perseguir
            shouldEvade = false;
            Debug.Log("Jugador sin arma detectado: Persiguiendo");
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
