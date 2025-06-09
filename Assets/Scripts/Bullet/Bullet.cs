using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float lifetime;
    private float lifeTimer;
    public void Initialize(Transform targetTransform, float bulletSpeed, float bulletLifetime)
    {
        target = targetTransform;
        speed = bulletSpeed;
        lifetime = bulletLifetime;
        lifeTimer = 0f;
    }
    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.LookAt(target);
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.KillPlayer();
                Destroy(gameObject);
            }
        }
    }
}