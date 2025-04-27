using UnityEngine;

public class WeaponPickUp : MonoBehaviour, Iinteractable
{
    public GameObject Weapon;
    private PlayerController player;
    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
    }
    public void interaction()
    {
        if (player != null && Weapon != null)
        {
            player.EquipWeapon(Weapon);
            gameObject.SetActive(false);
            Debug.Log("Picked Object");
        }
        else
        {
            Debug.LogWarning("Either is not working or there is no Object assigned to the Pick Up ;p");
        }
    }
}