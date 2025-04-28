using UnityEngine;

public class RangedEnemysIdle<T> : RangedEnemysBase<T>
{
    private float _currentAngle;

    public override void Enter()
    {
        base.Enter();
        _move.Move(Vector3.zero);
        _currentAngle = 0f; // Reseteamos el ángulo al entrar al idle
    }

    public override void Execute()
    {
        base.Execute();

        // Aumentamos el ángulo con el tiempo
        _currentAngle += 30f * Time.deltaTime; // 30 grados por segundo (ajustalo como quieras)

        // Convertimos el ángulo a un vector de dirección en el plano XZ
        Vector3 dir = new Vector3(Mathf.Cos(_currentAngle), 0, Mathf.Sin(_currentAngle));

        // Aplicamos la dirección de mirada
        _look.LookDir(dir.normalized);
    }


}