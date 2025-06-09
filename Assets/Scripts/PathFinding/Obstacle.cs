using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void Start()
    {
        var coll = GetComponent<Collider>();
        if (ObstacleManager.Instance != null)
        {
            ObstacleManager.Instance.AddColl(coll);
        }
    }

    private void OnDestroy()
    {
        if (ObstacleManager.Instance != null)
        {
            var coll = GetComponent<Collider>();
            ObstacleManager.Instance.RemoveColl(coll);
        }
    }
}