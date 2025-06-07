using UnityEngine;

public class WeaponStack2
{
    public Weapon weapon;
    public int count;

    public WeaponStack2(Weapon weapon)
    {
        this.weapon = weapon;
        this.count = 1;
    }
}