using System;
using UnityEngine;

[Serializable]
public struct StateFlockingConfig
{
    public string stateName;
    public FlockingBehaviourConfig[] behaviourConfigs;

    public StateFlockingConfig(string name, params FlockingBehaviourConfig[] configs)
    {
        stateName = name;
        behaviourConfigs = configs;
    }
}