using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Bullet2 : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    

    private float lifeTimer = 0f;

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                EventManager.InvokeNPCDeath(damageable);
                var _counterManager = ServiceLocator.Instance.GetService<CounterManager>();
                CounterManager.Instance.RegisterKill("Weapon 2");
            }

            Destroy(gameObject); 
        }
    }
}