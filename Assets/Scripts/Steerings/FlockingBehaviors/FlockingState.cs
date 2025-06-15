using UnityEngine;

[System.Serializable]
public struct FlockingState
{
    public bool isActive;
    public float multiplier;

    public FlockingState(bool active, float mult)
    {
        isActive = active;
        multiplier = mult;
    }
}
