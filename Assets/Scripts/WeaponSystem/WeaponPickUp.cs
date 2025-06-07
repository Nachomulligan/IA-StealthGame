using UnityEngine;


public class WeaponPickUp : MonoBehaviour, Iinteractable
{
    public Weapon weaponData;

    private PlayerController playerController;

    private void Start()
    {
        playerController = ServiceLocator.Instance.GetService<PlayerController>();
    }

    public void interaction()
    {
        if (playerController != null && weaponData != null)
        {
            playerController.PickUpWeapon(weaponData);

            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Either PlayerController is not working or there is no WeaponData assigned to the Pick Up");
        }
    }
}