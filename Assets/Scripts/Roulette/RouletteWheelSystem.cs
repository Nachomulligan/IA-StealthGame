using System.Collections.Generic;
using UnityEngine;

public class RouletteWheelSystem : MonoBehaviour
{
    private static RouletteWheelSystem _instance;
    public static RouletteWheelSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("RouletteWheelSystem");
                _instance = go.AddComponent<RouletteWheelSystem>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    //singleton
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public float Range(float min, float max)
    {
        return min + Random.value * (max - min);
    }

    public T Roulette<T>(Dictionary<T, float> items)
    {
        float total = 0;
        foreach (var item in items)
        {
            total += item.Value;
        }

        float random = Range(0, total);

        foreach (var item in items)
        {
            if (random <= item.Value)
            {
                return item.Key;
            }
            else
            {
                random -= item.Value;
            }
        }

        return default(T);
    }

    /// <summary>
    /// Versión simplificada para elegir entre dos opciones con porcentajes dados
    /// </summary>
    /// <returns>La opción seleccionada</returns>
    public T RouletteChoice<T>(T option1, float weight1, T option2, float weight2)
    {
        var options = new Dictionary<T, float>
        {
            { option1, weight1 },
            { option2, weight2 }
        };

        return Roulette(options);
    }

    /// <summary>
    /// Versión específica para el comportamiento de evasión o persecución
    /// </summary>
    /// <returns>True si debe evadir, False si debe perseguir</returns>
    public bool ShouldEvadeTarget(float chaseWeight, float evadeWeight)
    {
        var options = new Dictionary<bool, float>
        {
            { false, chaseWeight }, // false = perseguir
            { true, evadeWeight }   // true = evadir
        };

        return Roulette(options);
    }
}
