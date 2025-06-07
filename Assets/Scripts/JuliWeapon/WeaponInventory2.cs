using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory2 : MonoBehaviour
{
    public List<WeaponStack> weaponStacks = new List<WeaponStack>();

    public void AddWeapon(Weapon newWeapon)
    {
        WeaponStack existingStack = weaponStacks.Find(ws => ws.weapon == newWeapon);

        if (existingStack != null)
        {
            existingStack.count++;
            Debug.Log($"Weapon {newWeapon.weaponName} adds this value to stack: {existingStack.count}");
        }
        else
        {
            weaponStacks.Add(new WeaponStack(newWeapon));
            Debug.Log($"Weapon {newWeapon.weaponName} added to inventory");
        }
    }

    public Weapon GetWeaponToEquip(int index)
    {
        if (index >= 0 && index < weaponStacks.Count)
        {
            return weaponStacks[index].weapon;

        }
        return null;
    }
}

