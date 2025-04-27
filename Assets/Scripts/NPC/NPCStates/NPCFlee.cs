using UnityEngine;

public class NPCFlee<T> : NPCSBase<T>
{
    ISteering _steering;

    public NPCFlee(ISteering steering)
    {
        _steering = steering;
    }

    public override void Execute()
    {
        base.Execute();

        if (_steering == null)
        {
            Debug.LogWarning("Steering is NULL");
            return;
        }

        Vector3 direction = _steering.GetDir();
        Debug.Log($"[FLEE STATE] Direction: {direction}");

        if (_move == null)
        {
            Debug.LogWarning("_move is NULL");
            return;
        }
        _move.Move(direction);
    }
}