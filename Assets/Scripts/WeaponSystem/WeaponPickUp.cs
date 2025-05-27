using UnityEngine;

public class WeaponPickUp : MonoBehaviour, Iinteractable
{
    public GameObject Weapon;
    private PlayerController playerController;

    private void Start()
    {
        playerController = ServiceLocator.Instance.GetService<PlayerController>();
    }

    public void interaction()
    {
        if (playerController != null && Weapon != null)
        {
            playerController.EquipWeapon(Weapon);

            gameObject.SetActive(false);
            Debug.Log("Picked Object");
        }
        else
        {
            Debug.LogWarning("Either PlayerController is not working or there is no Weapon assigned to the Pick Up");
        }
    }
}
