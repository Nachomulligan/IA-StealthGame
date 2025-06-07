using System.Collections.Generic;
using UnityEngine;

public class Avoidance : FlockingBaseBehaviour
{
    public float personalArea = 0.5f;

    private void Awake()
    {
        if (_flockingType != FlockingType.Avoidance)
            _flockingType = FlockingType.Avoidance;
    }

    protected override Vector3 GetRealDir(List<IBoid> boids, IBoid self)
    {
        Vector3 avoidance = Vector3.zero;
        for (int i = 0; i < boids.Count; i++)
        {
            Vector3 diff = self.Position - boids[i].Position;
            if (diff.magnitude > personalArea) continue;

            avoidance += diff.normalized * (personalArea - diff.magnitude);
        }
        return avoidance.normalized * multiplier;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, personalArea);
    }
}