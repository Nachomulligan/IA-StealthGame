using UnityEngine;

public static class FlockingManagerExtensions
{
    public static void ApplyConfiguration(this FlockingManager flockingManager, FlockingConfiguration config, string stateName)
    {
        if (config == null)
        {
            Debug.LogError($"FlockingConfiguration es null! No se puede aplicar configuraci?n para el estado: {stateName}");
            return;
        }

        flockingManager.SaveCurrentState();
        config.ApplyStateConfiguration(stateName, flockingManager);
    }
}
