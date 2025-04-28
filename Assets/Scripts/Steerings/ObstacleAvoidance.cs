using UnityEngine;
public class ObstacleAvoidance : MonoBehaviour
{
    [Min(1)]
    public int maxObs = 2;
    [Min(0)]
    public float radius;
    [Min(1)]
    public float angle;

    public float personalArea;
    public LayerMask obsMask;
    Collider[] _colls;
    private void Awake()
    {
        _colls = new Collider[maxObs];
    }
    public Vector3 GetDir(Vector3 currDir)
    {
        // Detect nearby obstacles
        int count = Physics.OverlapSphereNonAlloc(Self, radius, _colls, obsMask);

        Collider nearColl = null;
        float nearCollDistance = 0;
        Vector3 nearClosestPoint = Vector3.zero;

        // looks through detected obstacles
        for (int i = 0; i < count; i++)
        {
            Collider currColl = _colls[i];

            // Find the closest point on the obstacle 
            Vector3 closestPoint = currColl.ClosestPoint(Self);
            Vector3 dir = closestPoint - Self;
            float distance = dir.magnitude;

            // Check if the obstacle is within the allowed angle relative to current direction
            var currAngle = Vector3.Angle(dir, currDir);
            if (currAngle > angle / 2) continue;

            // Keep track of the nearest valid obstacle
            if (nearColl == null || distance < nearCollDistance)
            {
                nearColl = currColl;
                nearCollDistance = distance;
                nearClosestPoint = closestPoint;
            }
        }

        // If no valid obstacle is detected, continue moving in the current direction
        if (nearColl == null)
        {
            return currDir;
        }
        // Calculate avoidance direction
        Vector3 relativePos = transform.InverseTransformPoint(nearClosestPoint);
        Vector3 dirToColl = (nearClosestPoint - Self).normalized;
        Vector3 avoidanceDir = Vector3.Cross(transform.up, dirToColl);

        // Invert avoidance direction if the obstacle is on the right side
        if (relativePos.x > 0)
        {
            avoidanceDir = -avoidanceDir;
        }
        Debug.DrawRay(Self, avoidanceDir * 2, Color.red);

        // Blend current direction and avoidance direction based on obstacle proximity
        return Vector3.Lerp(currDir, avoidanceDir, (radius - Mathf.Clamp(nearCollDistance - personalArea, 0, radius)) / radius);
    }
    public Vector3 Self => transform.position;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, personalArea);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, angle / 2, 0) * transform.forward * radius);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -angle / 2, 0) * transform.forward * radius);
    }
}