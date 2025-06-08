using UnityEngine;

public class GoonEnemyModel : BaseFlockingEnemyModel, IBoid, ILook
{
    [Header("Goon Specific")]
    public float speedRot = 5;

    public Vector3 Forward => transform.forward;

    public void LookDir(Vector3 dir)
    {
        if (dir.x == 0 && dir.z == 0) return;
        transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * speedRot);
    }

    public override void Attack()
    {
        Debug.Log("Goon attacking!");
        _onAttack?.Invoke();
    }

}