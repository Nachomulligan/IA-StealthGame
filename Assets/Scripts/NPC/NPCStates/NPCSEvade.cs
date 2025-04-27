using UnityEngine;

public class NPCSEvade<T> : NPCSBase<T>
{
    ISteering _steering;

    public NPCSEvade(ISteering steering)
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
        Debug.Log($"[evade STATE] Direction: {direction}");

        if (_move == null)
        {
            Debug.LogWarning("_move is NULL");
            return;
        }
        _move.Move(direction);
    }
}