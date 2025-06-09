using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void Start()
    {
        var coll = GetComponent<Collider>();
        ObstacleManager.Instance.AddColl(coll);
    }
    private void OnDestroy()
    {
        var coll = GetComponent<Collider>();
        ObstacleManager.Instance.RemoveColl(coll);
    }
}
