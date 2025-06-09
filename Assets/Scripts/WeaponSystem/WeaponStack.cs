using UnityEngine;

public class WeaponStack
{
    public Weapon weapon;
    public int count;

    public WeaponStack(Weapon weapon)
    {
        this.weapon = weapon;
        this.count = 1;
    }
}