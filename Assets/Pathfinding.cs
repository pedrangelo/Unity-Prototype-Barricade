using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnhancedPathfindingAI : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform player;
    private int destPoint = 0;
    private NavMeshAgent agent;
    public float visionRange = 10f;
    public float visionAngle = 45f;
    public LayerMask obstacleLayers, playerLayer;
    public int tooManyObstaclesThreshold = 3;
    private bool isChasingPlayer = false;
    private float chaseTimer = 5f;
    private float chaseCooldown = 30f;
    private bool canChaseAgain = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        GoToNextPoint();
    }

    void GoToNextPoint()
    {
        if (waypoints.Length == 0 || isChasingPlayer)
            return;

        agent.destination = waypoints[destPoint].position;
        destPoint = (destPoint + 1) % waypoints.Length;
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isChasingPlayer)
            GoToNextPoint();

        DetectAndHandleObstacles();
        ChasePlayerLogic();
    }

    void DetectAndHandleObstacles()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, visionRange, obstacleLayers);
        int obstacleCount = 0;

        foreach (var hitCollider in hitColliders)
        {
            Vector3 directionToCollider = (hitCollider.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToCollider) < visionAngle / 2)
            {
                obstacleCount++;
                AssignNavMeshObstacle(hitCollider.gameObject);
            }
        }

        if (obstacleCount > tooManyObstaclesThreshold)
        {
            Debug.Log("Too many obstacles, recalculating path...");
            RecalculatePath();
        }
        else if (obstacleCount > 0)
        {
            Debug.Log("Few obstacles detected, attempting to pass...");
        }
    }

    void AssignNavMeshObstacle(GameObject obstacle)
    {
        if (!obstacle.GetComponent<NavMeshObstacle>())
        {
            var navObstacle = obstacle.AddComponent<NavMeshObstacle>();
            navObstacle.carving = true;
            StartCoroutine(RemoveNavMeshObstacleAfterDelay(obstacle, 30f));
        }
    }

    IEnumerator RemoveNavMeshObstacleAfterDelay(GameObject obstacle, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obstacle != null)
        {
            Destroy(obstacle.GetComponent<NavMeshObstacle>());
        }
    }

    void RecalculatePath()
    {
        GoToNextPoint();
    }

    void ChasePlayerLogic()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 dirToPlayer = (player.position - transform.position).normalized;

        if (canChaseAgain && Vector3.Angle(transform.forward, dirToPlayer) < visionAngle / 2 && distanceToPlayer < visionRange)
        {
            isChasingPlayer = true;
            agent.SetDestination(player.position);

            if (chaseTimer > 0)
            {
                chaseTimer -= Time.deltaTime;
            }
            else
            {
                isChasingPlayer = false;
                chaseTimer = 5f;
                StartCoroutine(ChaseCooldown());
            }
        }
    }

    IEnumerator ChaseCooldown()
    {
        canChaseAgain = false;
        yield return new WaitForSeconds(chaseCooldown);
        canChaseAgain = true;
    }

    void OnDrawGizmos()
    {
        // Visualization code remains the same as previously provided
    }
}
