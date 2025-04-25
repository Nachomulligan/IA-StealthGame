using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        //personalAreaGetComponent<Collider>().bounds.extents.magnitude;
    }
    public Vector3 GetDir(Vector3 currDir)
    {
        int count = Physics.OverlapSphereNonAlloc(Self, radius, _colls, obsMask);

        Collider nearColl = null;
        float nearCollDistance = 0;
        Vector3 nearClosestPoint = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            Collider currColl = _colls[i];
            Vector3 closestPoint = currColl.ClosestPoint(Self);
            Vector3 dir = closestPoint - Self;
            float distance = dir.magnitude;

            var currAngle = Vector3.Angle(dir, currDir);
            if (currAngle > angle / 2) continue;
            if (nearColl == null || distance < nearCollDistance)
            {
                nearColl = currColl;
                nearCollDistance = distance;
                nearClosestPoint = closestPoint;
            }
        }

        if (nearColl == null)
        {
            return currDir;
        }

        Vector3 relativePos = transform.InverseTransformPoint(nearClosestPoint);
        Vector3 dirToColl = (nearClosestPoint - Self).normalized;
        Vector3 avoidanceDir = Vector3.Cross(transform.up, dirToColl);
        if (relativePos.x > 0)
        {
            avoidanceDir = -avoidanceDir;
        }
        Debug.DrawRay(Self, avoidanceDir * 2, Color.red);

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
