using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float attackRange = 5f;
    public LayerMask enemyLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("Performed an attack");

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider enemyCollider in hitEnemies)
        {
            EnemyDeath enemy = enemyCollider.GetComponent<EnemyDeath>();
            if (enemy != null)
            {
                enemy.Die();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

