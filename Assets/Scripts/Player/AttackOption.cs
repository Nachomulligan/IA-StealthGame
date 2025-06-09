using UnityEngine;

public class AttackOption
{
    public string attackName;
    public AttackType type;
    public float weight;
}

public enum AttackType
{
    Melee,
    Ranged
}