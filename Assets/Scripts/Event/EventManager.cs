using UnityEngine;

public static class EventManager
{
    public delegate void NPCDeathEventHandler(NPCModel npc);
    public static event NPCDeathEventHandler OnNPCDeath;

    public static void InvokeNPCDeath(NPCModel npc)
    {
        OnNPCDeath?.Invoke(npc);
    }
}