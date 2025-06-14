using System.Collections.Generic;
using UnityEngine;

public class TargetTrackingService : MonoBehaviour
{
    private Dictionary<int, TargetData> _targetTracking = new Dictionary<int, TargetData>();

    private class TargetData
    {
        public float LastTimeSeen = float.NegativeInfinity;
        public float VisibilityThreshold;
        public Rigidbody Target;
        public LineOfSightMono LOS;
        public Transform Entity;
        public bool IsActive = true;
        public bool IsSearching = false; // Nuevo flag para saber si está en búsqueda
    }

    private void Awake()
    {
        // Registrar el servicio en el Service Locator
        ServiceLocator.Instance.Register<TargetTrackingService>(this);
    }

    private void Update()
    {
        foreach (var kvp in _targetTracking)
        {
            var data = kvp.Value;
            if (data.IsActive && data.IsSearching && data.Target != null && data.LOS != null)
            {
                if (data.LOS.LOS(data.Target.transform))
                {
                    data.LastTimeSeen = Time.time;
                }
            }
        }
    }

    public void RegisterTracker(int entityId, Rigidbody target, LineOfSightMono los, Transform entity, float visibilityThreshold = 2.5f)
    {
        _targetTracking[entityId] = new TargetData
        {
            Target = target,
            LOS = los,
            Entity = entity,
            VisibilityThreshold = visibilityThreshold,
            IsActive = true,
            IsSearching = false
        };
    }

    public void UnregisterTracker(int entityId)
    {
        if (_targetTracking.ContainsKey(entityId))
        {
            _targetTracking.Remove(entityId);
        }
    }

    public bool WasTargetSeenRecently(int entityId)
    {
        if (_targetTracking.TryGetValue(entityId, out var data))
        {
            return data.IsSearching && (Time.time - data.LastTimeSeen) < data.VisibilityThreshold;
        }
        return false;
    }

    public float GetLastTimeTargetSeen(int entityId)
    {
        if (_targetTracking.TryGetValue(entityId, out var data))
        {
            return data.LastTimeSeen;
        }
        return float.NegativeInfinity;
    }

    public void SetTrackerActive(int entityId, bool active)
    {
        if (_targetTracking.TryGetValue(entityId, out var data))
        {
            data.IsActive = active;
        }
    }

    // Nuevos métodos para controlar el modo búsqueda
    public void StartSearchMode(int entityId)
    {
        if (_targetTracking.TryGetValue(entityId, out var data))
        {
            data.IsSearching = true;
            // Si no tiene un tiempo inicial, usar el tiempo actual como referencia
            if (data.LastTimeSeen == float.NegativeInfinity)
            {
                data.LastTimeSeen = Time.time;
            }
        }
    }

    public void StopSearchMode(int entityId)
    {
        if (_targetTracking.TryGetValue(entityId, out var data))
        {
            data.IsSearching = false;
            data.LastTimeSeen = float.NegativeInfinity; // Resetear el timer
        }
    }

    public bool IsCurrentlySearching(int entityId)
    {
        if (_targetTracking.TryGetValue(entityId, out var data))
        {
            return data.IsSearching;
        }
        return false;
    }

    // Método para actualizar el timer cuando ve al target fuera del modo búsqueda
    public void UpdateLastSeenTime(int entityId)
    {
        if (_targetTracking.TryGetValue(entityId, out var data))
        {
            data.LastTimeSeen = Time.time;
        }
    }
}