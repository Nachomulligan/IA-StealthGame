using UnityEngine;

public class WeaponPickUp : MonoBehaviour, Iinteractable
{
    public GameObject Weapon;
    private PlayerController player;
    private PlayerModel playerModel; // <-- NUEVO

    private void Start()
    {
        player = ServiceLocator.Instance.GetService<PlayerController>();
        playerModel = ServiceLocator.Instance.GetService<PlayerModel>(); // <-- Buscamos al modelo
    }

    public void interaction()
    {
        if (player != null && Weapon != null)
        {
            player.EquipWeapon(Weapon);
            if (playerModel != null)
            {
                playerModel.EnableAttack(); // <-- Habilitamos ataque cuando recoge
            }

            gameObject.SetActive(false);
            Debug.Log("Picked Object");
        }
        else
        {
            Debug.LogWarning("Either is not working or there is no Object assigned to the Pick Up ;p");
        }
    }
}