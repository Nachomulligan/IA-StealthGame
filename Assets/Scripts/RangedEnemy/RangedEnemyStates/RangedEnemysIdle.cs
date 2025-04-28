using UnityEngine;

public class RangedEnemysIdle<T> : RangedEnemysBase<T>
{
    private float _currentAngle;

    public override void Enter()
    {
        base.Enter();
        _move.Move(Vector3.zero);
        _currentAngle = 0f; // Reseteamos el �ngulo al entrar al idle
    }

    public override void Execute()
    {
        base.Execute();

        // Aumentamos el �ngulo con el tiempo
        _currentAngle += 30f * Time.deltaTime; // 30 grados por segundo (ajustalo como quieras)

        // Convertimos el �ngulo a un vector de direcci�n en el plano XZ
        Vector3 dir = new Vector3(Mathf.Cos(_currentAngle), 0, Mathf.Sin(_currentAngle));

        // Aplicamos la direcci�n de mirada
        _look.LookDir(dir.normalized);
    }


}