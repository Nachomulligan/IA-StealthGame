using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/New Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;
}