using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CounterManager : MonoBehaviour
{
    public static CounterManager Instance;

    private Dictionary<string, int> weaponKillCount = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        ServiceLocator.Instance.Register<CounterManager>(this);
    }
    public void RegisterKill(string weaponName)
    {
        if (!weaponKillCount.ContainsKey(weaponName))
        {
            weaponKillCount[weaponName] = 0;
        }

        weaponKillCount[weaponName]++;
        Debug.Log($"Kill With Weapon: {weaponName}. Total: {weaponKillCount[weaponName]}");
    }
    public int GetTotalKills()
    {
        return weaponKillCount.Values.Sum();
    }

    public int GetKillsForWeapon(string weaponName)
    {
        if (weaponKillCount.ContainsKey(weaponName))
        {
            return weaponKillCount[weaponName];
        }
        return 0;
    }
}