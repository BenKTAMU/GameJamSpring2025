
using UnityEngine;
using Pathfinding;

public class EnemyPatrolManual : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    private int currentWaypointIndex = 0;
    private bool isPatrolling = true;
    private AIPath aiPath;

    public Transform player;
    public float detectionRange = 5f;
    public LayerMask obstacleMask;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        if (aiPath != null)
        {
            aiPath.enabled = false;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, obstacleMask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                StartChase(player);
            }
        }

        if (isPatrolling && waypoints.Length > 0)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            transform.position = Vector2.MoveTowards(transform.position,
                targetWaypoint.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
        }
    }

    public void StartChase(Transform playerObject)
    {
        isPatrolling = false;
        if (aiPath != null)
        {
            aiPath.enabled = true;
            aiPath.destination = playerObject.position;
        }
    }

    public void StopChase()
    {
        isPatrolling = true;
        if (aiPath != null)
        {
            aiPath.enabled = false;
        }
    }
}