using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlockingConfiguration", menuName = "AI/Flocking Configuration")]
public class FlockingConfiguration : ScriptableObject
{
    [Header("Estado Configurations")]
    [SerializeField] private StateFlockingConfig[] stateConfigs;

    private Dictionary<string, Dictionary<FlockingType, FlockingBehaviourConfig>> _configDict;

    private void OnEnable()
    {
        BuildConfigDictionary();
    }

    private void BuildConfigDictionary()
    {
        _configDict = new Dictionary<string, Dictionary<FlockingType, FlockingBehaviourConfig>>();

        foreach (var stateConfig in stateConfigs)
        {
            var behaviourDict = new Dictionary<FlockingType, FlockingBehaviourConfig>();

            foreach (var behaviourConfig in stateConfig.behaviourConfigs)
            {
                behaviourDict[behaviourConfig.type] = behaviourConfig;
            }

            _configDict[stateConfig.stateName] = behaviourDict;
        }
    }

    public bool TryGetStateConfig(string stateName, out Dictionary<FlockingType, FlockingBehaviourConfig> config)
    {
        if (_configDict == null)
            BuildConfigDictionary();

        return _configDict.TryGetValue(stateName, out config);
    }

    public void ApplyStateConfiguration(string stateName, FlockingManager flockingManager)
    {
        if (!TryGetStateConfig(stateName, out var config))
        {
            Debug.LogWarning($"No se encontró configuración para el estado: {stateName}");
            return;
        }

        foreach (var kvp in config)
        {
            var flockingType = kvp.Key;
            var behaviourConfig = kvp.Value;

            flockingManager.SetFlockingActive(flockingType, behaviourConfig.isActive);
            flockingManager.SetFlockingMultiplier(flockingType, behaviourConfig.multiplier);
        }
    }

    public FlockingBehaviourConfig GetBehaviourConfig(string stateName, FlockingType flockingType)
    {
        if (TryGetStateConfig(stateName, out var config) && config.TryGetValue(flockingType, out var behaviourConfig))
        {
            return behaviourConfig;
        }

        // Retorna configuración por defecto si no se encuentra
        return new FlockingBehaviourConfig(flockingType, false, 0f);
    }

    // Métodos de utilidad para el editor
    [ContextMenu("Add Default Configurations")]
    private void AddDefaultConfigurations()
    {
        stateConfigs = new StateFlockingConfig[]
        {
            new StateFlockingConfig("Patrol",
                new FlockingBehaviourConfig(FlockingType.Cohesion, true, 1f),
                new FlockingBehaviourConfig(FlockingType.Alignment, true, 1f),
                new FlockingBehaviourConfig(FlockingType.Avoidance, true, 3f),
                new FlockingBehaviourConfig(FlockingType.Leader, true, 15f),
                new FlockingBehaviourConfig(FlockingType.Predator, false, 0f),
                new FlockingBehaviourConfig(FlockingType.Seek, false, 0f)
            ),
            new StateFlockingConfig("Evade",
                new FlockingBehaviourConfig(FlockingType.Predator, true, 10f),
                new FlockingBehaviourConfig(FlockingType.Avoidance, true, 2f),
                new FlockingBehaviourConfig(FlockingType.Leader, false, 0f),
                new FlockingBehaviourConfig(FlockingType.Cohesion, false, 0f),
                new FlockingBehaviourConfig(FlockingType.Alignment, false, 0f),
                new FlockingBehaviourConfig(FlockingType.Seek, false, 0f)
            ),
            new StateFlockingConfig("GoZone",
                new FlockingBehaviourConfig(FlockingType.Seek, true, 1f),
                new FlockingBehaviourConfig(FlockingType.Alignment, true, 1f),
                new FlockingBehaviourConfig(FlockingType.Cohesion, true, 1f),
                new FlockingBehaviourConfig(FlockingType.Leader, false, 0f),
                new FlockingBehaviourConfig(FlockingType.Predator, false, 0f),
                new FlockingBehaviourConfig(FlockingType.Avoidance, false, 0f)
            ),
            new StateFlockingConfig("Idle",
                new FlockingBehaviourConfig(FlockingType.Leader, false, 0f),
                new FlockingBehaviourConfig(FlockingType.Predator, false, 0f),
                new FlockingBehaviourConfig(FlockingType.Seek, false, 0f),
                new FlockingBehaviourConfig(FlockingType.Alignment, false, 0f),
                new FlockingBehaviourConfig(FlockingType.Cohesion, false, 0f),
                new FlockingBehaviourConfig(FlockingType.Avoidance, false, 0f)
            )
        };

        BuildConfigDictionary();
    }
}
