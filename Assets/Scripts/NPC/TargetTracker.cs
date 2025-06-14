using System.Collections.Generic;
using UnityEngine;

public class TargetTracker : MonoBehaviour
{
    [Header("Target Tracking")]
    [SerializeField] private float visibilityThreshold = 2.5f;

    private float lastTimeSeen = float.NegativeInfinity;
    private bool isSearching = false;
    private Rigidbody target;
    private LineOfSightMono los;

    private void Awake()
    {
        var controller = GetComponent<BaseEnemyController>();
        if (controller != null)
        {
            target = controller.target;
            los = GetComponent<LineOfSightMono>();
        }
    }

    private void Update()
    {
        // Solo hacer el check si estamos en modo búsqueda y tenemos target
        if (isSearching && target != null && los != null)
        {
            if (los.LOS(target.transform))
            {
                lastTimeSeen = Time.time;
            }
        }
    }

    public bool WasTargetSeenRecently()
    {
        return isSearching && (Time.time - lastTimeSeen) < visibilityThreshold;
    }

    public float GetLastTimeTargetSeen()
    {
        return lastTimeSeen;
    }

    public void StartSearchMode()
    {
        isSearching = true;
        // Si no tiene un tiempo inicial, usar el tiempo actual como referencia
        if (lastTimeSeen == float.NegativeInfinity)
        {
            lastTimeSeen = Time.time;
        }
    }

    public void StopSearchMode()
    {
        isSearching = false;
        lastTimeSeen = float.NegativeInfinity; // Resetear el timer
    }

    public bool IsCurrentlySearching()
    {
        return isSearching;
    }

    public void UpdateLastSeenTime()
    {
        lastTimeSeen = Time.time;
    }

    public void SetVisibilityThreshold(float threshold)
    {
        visibilityThreshold = threshold;
    }
}