using UnityEngine;

public class Trapp : MonoBehaviour
{
    
    private  int currentTrapCount = 0;

    [SerializeField] private int maxTrapCount = 5; 

    private void Start()
    {
        if (currentTrapCount >= maxTrapCount)
        {
            Destroy(gameObject);
            return;
        }

        // Sumar esta trampa
        currentTrapCount++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                EventManager.InvokeNPCDeath(damageable);
            }

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (currentTrapCount > 0)
        {
            currentTrapCount--;
        }
    }
}