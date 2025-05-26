using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class EnemyAI : MonoBehaviour
{

    private NavMeshAgent agent;

    private Animator animator;

    public Transform player;
    public float speed = 3.5f;
    public float chaseRange = 10f;
    public int damage = 10;
    public float attackRate = 1f;
    public float fixedHeight = 1.5f;


    [Header("Roaming Settings")]
    public bool enableRoaming = true;
    public float roamingRange = 15f;
    public float roamingSpeed = 2f;
    public float roamingPauseTime = 2f;
    public float roamingWaypointThreshold = 1f;

    [Header("Path Following Settings")]
    public float waypointSpacing = 1f; // Minimum distance to place new waypoints
    public int maxWaypoints = 20; // Maximum waypoints to store

    [Header("Line of Sight")]
    public float sightRange = 20f;
    public float fieldOfView = 120f; // degrees
    public LayerMask obstacleMask;

    private Queue<Vector3> playerWaypoints = new Queue<Vector3>();
    private Vector3 roamingTarget;
    private bool isChasing = false;
    private float roamingPauseTimer = 0f;
    private float nextAttackTime = 0f;

    private EnemyHealth enemyHealth;
    private PlayerHealth playerHealth;

    void Start()
    {
       

        enemyHealth = GetComponent<EnemyHealth>();
        playerHealth = player.GetComponent<PlayerHealth>();
        SetNewRoamingTarget();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // We'll handle look direction manually
        agent.updateUpAxis = false;   // For 2D or top-down games

        agent.updateRotation = true;
        agent.angularSpeed = 360f; // Default is 120. Increase for faster turning.

        
        animator = GetComponent<Animator>(); // Gets the Animator attached to the enemy
    }

    void Update()
    {

        agent.speed = 6f;

        if (enemyHealth.currentHealth <= 0) return;

        Vector3 pos = transform.position;
        pos.y = fixedHeight;
        transform.position = pos;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (IsPlayerInSight() || distanceToPlayer <= chaseRange)
        {
            isChasing = true;
            agent.speed = speed;
            //UpdatePlayerWaypoints();
            //FollowPath();
            agent.SetDestination(player.position);
            Debug.Log("IsChasing: ");
            if (animator != null) {
                animator.SetBool("IsChasing", true);
                Debug.Log("IsChasing: 1");
            }
                
        }
        else
        {
            isChasing = false;
            if (enableRoaming)
                Roam();

            if (animator != null)
            {
                Debug.Log("IsChasing: 1");
                animator.SetBool("IsChasing", false);
            }
                

        }


    }



    void UpdatePlayerWaypoints()
    {
        // Add the player's current position to the waypoints if far enough from the last waypoint
        if (playerWaypoints.Count == 0 || Vector3.Distance(player.position, playerWaypoints.Peek()) > waypointSpacing)
        {
            playerWaypoints.Enqueue(player.position);

            // Remove the oldest waypoint if we exceed the maximum number
            if (playerWaypoints.Count > maxWaypoints)
            {
                playerWaypoints.Dequeue();
            }
        }
    }

    void FollowPath()
    {


        if (agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            Vector3 steeringDirection = (agent.steeringTarget - transform.position).normalized;
            transform.LookAt(new Vector3(transform.position.x + steeringDirection.x, fixedHeight, transform.position.z + steeringDirection.z));
        }


        if (playerWaypoints.Count > 0)
        {
            Vector3 targetWaypoint = playerWaypoints.Peek();

            // Move toward the waypoint using NavMesh
            agent.SetDestination(targetWaypoint);

            // Face movement direction
            Vector3 direction = (agent.steeringTarget - transform.position).normalized;
            if (direction.sqrMagnitude > 0.01f)
            {
                transform.LookAt(new Vector3(transform.position.x + direction.x, fixedHeight, transform.position.z + direction.z));
            }

            // Check if close enough to consider waypoint reached
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                playerWaypoints.Dequeue();
            }
        }
    }


    void Roam()
    {

        agent.speed = roamingSpeed * 2;
        agent.SetDestination(roamingTarget);

        float distanceToTarget = Vector3.Distance(transform.position, roamingTarget);

        if (distanceToTarget < roamingWaypointThreshold)
        {
            roamingPauseTimer += Time.deltaTime;

            if (roamingPauseTimer >= roamingPauseTime)
            {
                SetNewRoamingTarget();
                roamingPauseTimer = 0f;
            }
        }
        else
        {
            Vector3 direction = (roamingTarget - transform.position).normalized;

            // Move toward the roaming target
            //transform.position += direction * roamingSpeed * Time.deltaTime;

            // Keep enemy at a fixed height
            Vector3 position = transform.position;
            position.y = fixedHeight;
            transform.position = position;

            // Make the enemy face the roaming target
            transform.LookAt(new Vector3(roamingTarget.x, transform.position.y, roamingTarget.z));
        }
    }

    void SetNewRoamingTarget()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-roamingRange, roamingRange),
            0,
            Random.Range(-roamingRange, roamingRange)
        );

        roamingTarget = transform.position + randomDirection;
        roamingTarget.y = fixedHeight;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / attackRate;
            playerHealth.TakeDamage(damage);

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }


    }

    bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= sightRange && angleToPlayer < fieldOfView * 0.5f)
        {
            // Check for obstruction
            if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, directionToPlayer, distanceToPlayer, obstacleMask))
            {
                return true;
            }
        }

        return false;
    }
}
