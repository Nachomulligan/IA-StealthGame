using System.Collections.Generic;
using UnityEngine;

public class GoonManager : MonoBehaviour
{
    private List<GoonEnemyModel> _registeredGoons = new List<GoonEnemyModel>();

    private void Awake()
    {
        // Registrar el GoonManager en el ServiceLocator
        ServiceLocator.Instance.Register<GoonManager>(this);
    }

    public void RegisterGoon(GoonEnemyModel goon)
    {
        if (!_registeredGoons.Contains(goon))
        {
            _registeredGoons.Add(goon);
            Debug.Log($"Goon registered: {goon.name}");
        }
    }

    public void UnregisterGoon(GoonEnemyModel goon)
    {
        if (_registeredGoons.Contains(goon))
        {
            _registeredGoons.Remove(goon);
            Debug.Log($"Goon unregistered: {goon.name}");
        }
    }

    public List<GoonEnemyModel> GetAllGoons()
    {
        // Limpiar referencias nulas
        _registeredGoons.RemoveAll(goon => goon == null);
        return new List<GoonEnemyModel>(_registeredGoons);
    }

    public bool HasEvadingGoonInSight(LineOfSightMono lineOfSight)
    {
        foreach (var goon in _registeredGoons)
        {
            if (goon == null) continue;

            // Verificar si el goon est? en estado evading
            if (goon.IsEvading)
            {
                // Verificar si el leader puede ver al goon
                if (lineOfSight.LOS(goon.transform))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnDestroy()
    {
        _registeredGoons.Clear();
    }
}