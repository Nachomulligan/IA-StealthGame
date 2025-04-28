using UnityEngine;

public static class EventManager
{
    public delegate void NPCDeathEventHandler(IDamageable npc);
    public static event NPCDeathEventHandler OnNPCDeath;

    public static void InvokeNPCDeath(IDamageable npc)
    {
        OnNPCDeath?.Invoke(npc);
    }
}
