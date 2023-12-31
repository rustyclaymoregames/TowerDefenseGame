using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RedKnightController : MonoBehaviour
{
    NavMeshAgent agent;
    
    private Rigidbody2D rb;

    private Animator anim;
    [SerializeField] private float speed; //The speed of the knight

    [SerializeField] public Transform targetWaypoint; //The waypoint the enemy is moving towards

    private Transform originalSpawnLocation;

    private float attackTime = 0.3f; //The time it takes to attack
    private float attackTimeCounter = 0.3f; //The time it takes to attack
    private bool isAttacking; //Check if the knight is attacking
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //Get the rigidbody component 
        anim = GetComponent<Animator>(); //Get the animator component

        agent = GetComponent<NavMeshAgent>(); //Get the NavMeshAgent component
        agent.updateRotation = false; //Stop the NavMeshAgent component from rotating the enemy
        agent.updateUpAxis = false; //Stop the NavMeshAgent component from rotating the enemy

    }

    void Update()
    {
        MoveToTargetWaypoint();

        Vector2 normalizedVelocity = agent.velocity.normalized; //Get the normalized velocity of the enemy
        anim.SetFloat("moveX", normalizedVelocity.x); //Set the moveX parameter in the animator
        anim.SetFloat("moveY", normalizedVelocity.y); //Set the moveY parameter in the animator


        if (agent.velocity.magnitude == 0)
        {
            anim.SetBool("isMoving", false); // Set isMoving to false to stop the animation
        }
        else
        {
            anim.SetBool("isMoving", true); // Set isMoving to true to start the animation
        }

        if (agent.velocity.magnitude > 0f)
        {
            anim.SetFloat("lastMoveX", agent.velocity.x); //Set the lastMoveX parameter in the animator
            anim.SetFloat("lastMoveY", agent.velocity.y); //Set the lastMoveY parameter in the animator
        }

        if (isAttacking)
        {
            rb.velocity = Vector2.zero; //Set the velocity to zero
            attackTimeCounter -= Time.deltaTime; //Decrement the attack time counter
            if (attackTimeCounter <= 0)
            {
                anim.SetBool("isAttacking", false); //Set the isAttacking parameter in the animator
                isAttacking = false; //Set isAttacking to false
            }
        }

        //check if the attack animation is playing
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            //check if the animation has finished playing
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                anim.SetBool("isAttacking", false); //Set the isAttacking parameter in the animator
                isAttacking = false; //Set isAttacking to false
            }
        }
    }

    public void InitiateAttack()
    {
        attackTimeCounter = attackTime; //wait for counter to be up before attacking
        anim.SetBool("isAttacking", true); //Set the isAttacking parameter in the animator
        isAttacking = true; //Set isAttacking to true
    }

    public void StopAttack()
    {
        anim.SetBool("isAttacking", false); //Set the isAttacking parameter in the animator
        attackTimeCounter = 0;
        isAttacking = false;
    }

    public void SetTarget(GameObject inputTargetWaypoint)
    {
        // Set the targetWaypoint to the desired position
        try 
        {
            targetWaypoint = inputTargetWaypoint.transform;
        }
        catch (Exception e)
        {};
    }
    
    public void SetTarget(Transform inputTargetWaypoint)
    {
        // Set the targetWaypoint to the desired position
        try 
        {
            targetWaypoint = inputTargetWaypoint;
        }
        catch (Exception e)
        {};
    }

    void MoveToTargetWaypoint()
    {
        FollowWaypoint(targetWaypoint);
    }

    void FollowWaypoint(Transform waypoint)
    {
        if (waypoint != null)
        {
            agent.SetDestination(waypoint.position); //Set the destination of the NavMeshAgent component to the waypoint position

            if (agent.remainingDistance <= agent.stoppingDistance) //Check if the enemy has reached the target
            { 
                anim.SetBool("isMoving", false); // Set isMoving to false to stop the animation
                anim.SetFloat("moveX", 0f);
                anim.SetFloat("moveY", 0f);
            }
        }
    }


    public Transform GetOriginalSpawnLocation() 
    {
        return originalSpawnLocation;
    }
}