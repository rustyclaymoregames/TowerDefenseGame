using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public int damageAmount = 1; // The damage the arrow will do to the enemy

    private Rigidbody2D arrowRigidbody; // The arrow's rigidbody component

    public float range = 1.0f; // The range of the arrow. Will be time based. Was 2 before
    private float timer = 0.0f; // The timer for the arrow which will countdown the range
    private Vector3 initialDirection; // The initial direction of the arrow


    private void Start()
    {
        arrowRigidbody = GetComponent<Rigidbody2D>(); // Get the arrow's rigidbody component
        timer = range; // Set the timer to the range

    }

    void FixedUpdate() 
    {
        timer -= Time.deltaTime; // Decrement the timer
        if (timer <= 0.0f) // If the timer is less than or equal to 0
        {
            Destroy(gameObject); // Destroy the arrow
        }
    }


    public void ArrowBehaviour(Transform targetWaypoint, bool shouldFire, List<GameObject> inactiveArrows, float arrowSpeed)
    {
        //Similar logic to the TowerArcher.cs script of finding the closest enemy and dealing damage to it
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Get all the enemies on the scene
        GameObject closestEnemy = null; // The closest enemy
        float closestDistance = Mathf.Infinity; // The distance to the closest enemy
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position); // Calculate the distance between the tower and the enemy
                if (distance < closestDistance) // If the distance is less than the closest distance
                {
                    closestDistance = distance; // Set the closest distance to the distance
                    closestEnemy = enemy; // Set the closest enemy to the enemy
                }
            }
        }
        if (closestEnemy != null)
        {
            UnityEngine.AI.NavMeshAgent enemyAgent = closestEnemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (enemyAgent != null && targetWaypoint != null)
            {
                if (Vector3.Distance(enemyAgent.transform.position, targetWaypoint.position) <= 1.2f)
                {
                    // Stop firing when the enemy reaches the waypoint
                    shouldFire = false;
                    inactiveArrows.Add(gameObject);
                    Destroy(gameObject);
                }
                else if (Vector3.Distance(transform.position, enemyAgent.transform.position) <= 0.5f)
                {
                    // Handle the arrow hitting the enemy
                    DealDamage(closestEnemy);
                    inactiveArrows.Add(gameObject);
                    Destroy(gameObject);
                }
                else if (shouldFire)
                {
                    transform.position += initialDirection * arrowSpeed * Time.deltaTime;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            DealDamage(collision.gameObject);
            // inactiveArrows.Add(gameObject);
            Destroy(gameObject);
        }
    }


    public void DealDamage(GameObject enemy)
    {
        // GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            EnemyHealthZombie enemyHealthManager = enemy.GetComponent<EnemyHealthZombie>();
            if (enemyHealthManager != null)
            {
                enemyHealthManager.TakeDamage(1); // Change the value if needed
            }

            //@TODO: Remove this later as the health managers will merge
            EnemyHealthGoblinRider enemyHealthBat = enemy.GetComponent<EnemyHealthGoblinRider>();
            if (enemyHealthBat != null)
            {
                enemyHealthBat.TakeDamage(2); // Change the value if needed
            }
            EnemyHealthNecromancer enemyHealthNecromancer = enemy.GetComponent<EnemyHealthNecromancer>();
            if (enemyHealthNecromancer != null)
            {
                enemyHealthNecromancer.TakeDamage(2); // Change the value if needed
            }
            EnemyHealthAncientSkeleton enemyHealthAncientSkeleton = enemy.GetComponent<EnemyHealthAncientSkeleton>();
            if (enemyHealthAncientSkeleton != null)
            {
                enemyHealthAncientSkeleton.TakeDamage(1); // Change the value if needed
            }
        }
    }
}
