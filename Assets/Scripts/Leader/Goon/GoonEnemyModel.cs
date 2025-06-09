using System.Collections.Generic;
using UnityEngine;

public class GoonEnemyModel : BaseFlockingEnemyModel, IBoid, ILook
{
    [Header("Goon Specific")]
    public float speedRot = 5;

    private GoonManager _goonManager;
    private bool _isEvading;

    public Vector3 Forward => transform.forward;
    public bool IsEvading => _isEvading;

    protected override void Awake()
    {
        base.Awake();
        _goonManager = ServiceLocator.Instance.GetService<GoonManager>();
    }

    public void Start()
    {
        // Registrarse en el GoonManager (responsabilidad del Model)
        if (_goonManager != null)
        {
            _goonManager.RegisterGoon(this);
        }
        else
        {
            Debug.LogWarning($"GoonManager not found for {gameObject.name}");
        }
    }

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

    // Método para que el Controller actualice el estado de evading
    public void SetEvadingState(bool isEvading)
    {
        _isEvading = isEvading;
    }

    public override void Die()
    {
        // Desregistrarse del GoonManager antes de morir (responsabilidad del Model)
        if (_goonManager != null)
        {
            _goonManager.UnregisterGoon(this);
        }

        base.Die(); // Llamar al método base que ya maneja la muerte
    }
}