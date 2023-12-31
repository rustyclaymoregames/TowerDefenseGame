using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TowerArcher : MonoBehaviour
{
    public GameObject arrowPrefab; // Add the arrow prefab here
    public Transform firePoint; // An empty game object as the fire point to assign in the inspector
    public Transform targetWaypoint; // The target waypoint the enemy moves towards

    [SerializeField] private float fireRate = 1.0f; // The rate of fire
    [SerializeField] private float arrowSpeed = 2f; // The speed of the arrow
    [SerializeField] private float firingRange = 5f; // The desired firing range

    private float timeUntilFire; // Timer to keep track of when to fire
    private List<GameObject> activeArrows = new List<GameObject>(); // List to hold active arrows
    private bool shouldFire = true; // A flag to determine if the tower should fire

    public LauncherWeapon launcherWeapon; // Reference to the LauncherWeapon script

    void Start()
    {
        timeUntilFire = 0f; // Set the timer to the fire rate

        // Manually assign the launcherWeapon reference
        launcherWeapon = GetComponentInChildren<LauncherWeapon>();
        if (launcherWeapon == null)
        {
            Debug.LogError("LauncherWeapon script not found on the same GameObject!");
        }
    }

    void Update()
    {
        // Countdown the timer until it's time to fire the next arrow
        timeUntilFire -= Time.deltaTime;

        // If the timer reaches zero or below, fire an arrow and reset the timer
        if (timeUntilFire <= 0 && shouldFire)
        {
            FireArrow();
            timeUntilFire = fireRate;
        }

        // Create a separate list to hold arrows that are no longer active
        List<GameObject> inactiveArrows = new List<GameObject>();

        // If an arrow is present, move it towards the enemy's position
        foreach (GameObject arrow in activeArrows)
        {
            if (arrow != null)
            {
                ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
                if (arrowScript != null)
                {
                    arrowScript.ArrowBehaviour(targetWaypoint, shouldFire, inactiveArrows, arrowSpeed);
                }
            }
        }

        // Remove the inactive arrows from the active arrows list
        foreach (GameObject arrow in inactiveArrows)
        {
            activeArrows.Remove(arrow);
        }
    }

    private void FireArrow()
    {
        // The Tower should find the closest enemy before firing an arrow. If there are multiple enemies on the scene it will prioritize the nearest one.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Get all the enemies on the scene
        GameObject closestEnemy = null; // The closest enemy
        float closestDistance = Mathf.Infinity; // The distance to the closest enemy
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position); // Calculate the distance between the tower and the enemy
                if (distance < closestDistance && distance <= firingRange) // If the distance is less than the closest distance
                {
                    closestDistance = distance; // Set the closest distance to the distance
                    closestEnemy = enemy; // Set the closest enemy to the enemy
                }
            }
        }
        if (closestEnemy != null)
        {
            NavMeshAgent enemyAgent = closestEnemy.GetComponent<NavMeshAgent>();
            launcherWeapon.PlayAttackAnimation();
            ShootArrow(enemyAgent);
        }
        else
        {
            launcherWeapon.PlayIdleAnimation();
        }
    }


    public void ShootArrow(NavMeshAgent enemyAgent)
    {
            // Instantiate an arrow prefab at the fire point position and rotation
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

            // Calculate the direction towards the enemy agent
            Vector3 direction = (enemyAgent.transform.position - firePoint.position).normalized;

            arrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction) * Quaternion.Euler(0, 0, 270);

            // Get the arrow component
            Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();

            // Set the initial velocity of the arrow towards the enemy
            if (rb != null)
            {
                rb.velocity = direction * arrowSpeed;
                activeArrows.Add(arrow); // Add the arrow to the list of active arrows
            }
    }
}
