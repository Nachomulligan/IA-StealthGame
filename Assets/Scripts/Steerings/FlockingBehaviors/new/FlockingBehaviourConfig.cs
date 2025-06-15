using System;
using UnityEngine;

[Serializable]
public struct FlockingBehaviourConfig
{
    public FlockingType type;
    public bool isActive;
    [Min(0f)]
    public float multiplier;

    public FlockingBehaviourConfig(FlockingType flockingType, bool active = true, float mult = 1f)
    {
        type = flockingType;
        isActive = active;
        multiplier = mult;
    }
}