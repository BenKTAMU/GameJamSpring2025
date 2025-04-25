using UnityEngine;
using Pathfinding;
using System.Collections;


public class EnemyPatrolManual : MonoBehaviour
{
    // --- State Machine ---
    private enum AIState { Patrolling, Chasing, Distracted }
    private AIState currentState = AIState.Patrolling;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    public float patrolSpeed = 2f;
    public float rotationSpeed = 5f;
    private int currentWaypointIndex = 0;

    [Header("Chase Settings")]
    public Transform player;
    public float chaseSpeed = 7f;
    public float loseChaseDistance = 15f; 
    private AIPath aiPath;

    [Header("Vision Settings")]
    public float viewDistance = 5f; 
    [Range(0, 360)]
    public float viewAngle = 45f;   
    public LayerMask targetMask = 1; 
    public LayerMask obstacleMask;  

    [Header("Distraction Settings")]
    public float distractionListenRange = 10f; 
    public float distractionSpeed = 2.5f;     
    public float distractionInvestigateTime = 3f; 
    private Vector3 distractionPoint;
    private Coroutine distractionCoroutine;

    [Header("Visual Cone (Line Renderer)")]
    public Material lineMaterial;
    public Color coneColor = Color.yellow;
    public float coneLineWidth = 0.1f;
    [Min(1)] public int coneResolution = 20;

    // Private variables
    private LineRenderer lineRenderer;
    private Vector3 currentForwardDirection = Vector3.right;
    private Rigidbody2D rb;

    private bool hasPlayedSound;
    void Awake() 
    {
        aiPath = GetComponent<AIPath>();
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            rb.isKinematic = true; 
            rb.gravityScale = 0;   
        }
        else Debug.LogError("Rigidbody2D component not found.", this);

        if (aiPath == null) Debug.LogWarning("AIPath component not found.", this);
        if (lineRenderer == null) Debug.LogError("LineRenderer component not found.", this);
    }

    void Start()
    {
        if (aiPath != null)
        {
            aiPath.maxSpeed = chaseSpeed;
            aiPath.enableRotation = false;
            aiPath.rotationSpeed = rotationSpeed * 2f; 
            aiPath.pickNextWaypointDist = 0.5f;
            aiPath.endReachedDistance = 0.5f;
            aiPath.canMove = false; 
            aiPath.canSearch = false; 
        }

        SetupLineRenderer();
        currentState = AIState.Patrolling; 

        
        if (waypoints.Length > 0 && waypoints[0] != null)
        {
            Vector3 initialDirection = (waypoints[0].position - transform.position).normalized;
            currentForwardDirection = (initialDirection == Vector3.zero) ? GetDefaultForward() : initialDirection;
            RotateTowards(currentForwardDirection, 1000f); // Snap rotation
        }
        else
        {
            currentForwardDirection = GetDefaultForward();
        }
        UpdateLineRendererShape();
        lineRenderer.enabled = true; 
    }

    void Update()
    {
        // --- State Machine Logic ---
        switch (currentState)
        {
            case AIState.Patrolling:
                HandlePatrolState();
                break;

            case AIState.Chasing:
                HandleChaseState();
                break;

            case AIState.Distracted:
                HandleDistractedState();
                break;
        }
    }

  

    void HandlePatrolState()
    {
        // Ensure components are in the correct state for patrolling
        if (aiPath != null && aiPath.canMove)
        {
            aiPath.canMove = false;
            aiPath.canSearch = false;
            aiPath.SetPath(null); // Clear any existing path
        }
        if (!lineRenderer.enabled) lineRenderer.enabled = true;
        
        HandlePatrolMovementAndRotation();
        UpdateLineRendererShape(); // Keep cone visible and oriented
        DetectPlayerToStartChase(); // Check if player enters vision cone
    }

    void HandleChaseState()
    {
        // Ensure components are in the correct state for chasing
        if (aiPath != null && !aiPath.canMove)
        {
            aiPath.maxSpeed = chaseSpeed;
            aiPath.canMove = true;
            aiPath.canSearch = true;
        }
        if (lineRenderer.enabled) lineRenderer.enabled = false; // Hide cone when chasing

        // Perform chase actions
        HandleChaseMovementAndRotation();
        CheckStopChaseCondition(); // Check if we should give up the chase
    }

    void HandleDistractedState()
    {
        // Ensure components are in the correct state for distraction
        if (aiPath != null && aiPath.canMove)
        {
            aiPath.canMove = false;
            aiPath.canSearch = false;
            aiPath.SetPath(null);
        }

        // Perform distraction actions only if not already investigating at the point
        if (distractionCoroutine == null)
        {
            MoveTowardsDistractionPoint();
            CheckIfReachedDistractionPoint();
        }
        // Always allow checking for player during distraction
        DetectPlayerToStartChase();
    }


    // --- Patrol Logic ---

    void HandlePatrolMovementAndRotation()
    {
        CalculatePatrolDirection();
        PatrolMovement();
    }

    void CalculatePatrolDirection()
    {
        if (waypoints.Length > 0 && waypoints[currentWaypointIndex] != null)
        {
            Vector3 directionToWaypoint = (waypoints[currentWaypointIndex].position - transform.position);
            directionToWaypoint.z = 0; // Ensure 2D

            if (directionToWaypoint.sqrMagnitude > 0.01f)
            {
                currentForwardDirection = directionToWaypoint.normalized;
            }
            RotateTowards(currentForwardDirection, rotationSpeed);
        }
        else
        {
            // No waypoints, just stand still facing default direction
            currentForwardDirection = GetDefaultForward();
            RotateTowards(currentForwardDirection, rotationSpeed);
        }
    }

    void PatrolMovement()
    {
        if (waypoints.Length > 0 && waypoints[currentWaypointIndex] != null)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, patrolSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            }
        }
    }

    void DetectPlayerToStartChase()
    {
        if (player == null) return; // Can't chase null


        if (currentState != AIState.Chasing)
        {
             float distanceToPlayer = Vector3.Distance(transform.position, player.position);

             // Use current facing direction for check
             Vector3 directionToPlayer = (player.position - transform.position).normalized;

             if (distanceToPlayer <= viewDistance &&
                 Vector3.Angle(currentForwardDirection, directionToPlayer) < viewAngle / 2)
             {
                 // Line of sight check
                 RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, viewDistance, obstacleMask | targetMask);

                 // If player layer is hit first (no obstacles)
                 if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & targetMask) != 0)
                 {
                     if (!hasPlayedSound)
                     {
                         GameObject.Find("PlayerSpotted").GetComponent<AudioSource>().Play(); // Play sound
                         StartChase(player); // Player detected, switch to chase state
                         hasPlayedSound = true;
                     }

                 }
             }
        }
    }


    // --- Chase Logic ---

    void HandleChaseMovementAndRotation()
    {
        UpdateChaseTarget();
        
        if (aiPath != null && !aiPath.enableRotation && player != null) {
            RotateTowards((player.position - transform.position).normalized, rotationSpeed * 1.5f);
        }
    }

    void UpdateChaseTarget()
    {
        if (player == null || aiPath == null || !aiPath.canSearch) return; // Stop if player disappears or AIPath disabled

        aiPath.destination = player.position;
    }

    void CheckStopChaseCondition()
    {
        if (player == null) // Player destroyed or unassigned
        {
            StopChase();
            return;
        }
        
        float distanceToPlayer = (aiPath != null && aiPath.hasPath)
            ? aiPath.remainingDistance
            : Vector3.Distance(transform.position, player.position);
        
        bool pathInvalid = aiPath != null && !aiPath.pathPending && (aiPath.reachedEndOfPath || !aiPath.hasPath);


        if (distanceToPlayer > loseChaseDistance || pathInvalid)
        {
            StopChase();
        }
    }

    void StartChase(Transform playerObject)
    {
        if (currentState == AIState.Chasing) return; // Already chasing

        Debug.Log("Starting Chase!");
        currentState = AIState.Chasing;
        player = playerObject; // Ensure the player reference is up-to-date

        // Stop any ongoing distraction investigation
        if (distractionCoroutine != null)
        {
            StopCoroutine(distractionCoroutine);
            distractionCoroutine = null;
        }

        // Enable AIPath for movement and searching
        if (aiPath != null)
        {
            aiPath.maxSpeed = chaseSpeed;
            aiPath.canMove = true;
            aiPath.canSearch = true;
            aiPath.destination = playerObject.position; // Set initial destination
            aiPath.SearchPath(); // Immediately search for a path
        }
        lineRenderer.enabled = false; // Hide vision cone
    }

    void StopChase()
    {
        if (currentState != AIState.Chasing) return; // Only stop if chasing

        Debug.Log("Stopping Chase, Resuming Patrol.");
        currentState = AIState.Patrolling;

        if (aiPath != null)
        {
            aiPath.canMove = false;
            aiPath.canSearch = false;
            aiPath.SetPath(null); // Clear the path
        }

        // Reset direction towards next waypoint or default
        CalculatePatrolDirection();
        lineRenderer.enabled = true;
        UpdateLineRendererShape();

    }

    // --- Distraction Logic ---
    
    public void HearDistraction(Vector3 impactPosition)
    {
        // Ignore distractions if currently chasing the player
        if (currentState == AIState.Chasing)
        {
            Debug.Log("Enemy is chasing, ignoring distraction.");
            return;
        }

        float distanceToImpact = Vector3.Distance(transform.position, impactPosition);

        if (distanceToImpact <= distractionListenRange)
        {
            Debug.Log("Enemy heard distraction at: " + impactPosition);
            distractionPoint = impactPosition;
            currentState = AIState.Distracted;
            
            if (distractionCoroutine != null)
            {
                StopCoroutine(distractionCoroutine);
                distractionCoroutine = null;
            }

            // Ensure AIPath is off
            if (aiPath != null)
            {
                aiPath.canMove = false;
                aiPath.canSearch = false;
                aiPath.SetPath(null);
            }
           // lineRenderer.enabled = false; 
        }
         else
        {
            Debug.Log($"Distraction at {impactPosition} too far ({distanceToImpact} > {distractionListenRange}).");
        }
    }

    void MoveTowardsDistractionPoint()
    {
        Vector3 directionToDistraction = (distractionPoint - transform.position);
        directionToDistraction.z = 0; 

        if (directionToDistraction.sqrMagnitude > 0.01f) //im really not sure what this does tbh
        {
            RotateTowards(directionToDistraction.normalized, rotationSpeed); 
        }

        transform.position = Vector2.MoveTowards(transform.position, distractionPoint, distractionSpeed * Time.deltaTime);
    }

    void CheckIfReachedDistractionPoint()
    {
        if (Vector2.Distance(transform.position, distractionPoint) < 0.8f) 
        {
            Debug.Log("Enemy reached distraction point. Investigating...");
            if (distractionCoroutine == null)
            {
                distractionCoroutine = StartCoroutine(InvestigateTimer());
            }
        }
    }

    IEnumerator InvestigateTimer()
    {
        yield return new WaitForSeconds(distractionInvestigateTime);

        Debug.Log("Investigation complete. Returning to patrol.");
        distractionCoroutine = null; // Mark coroutine as finished
        currentState = AIState.Patrolling; // Switch back to patrol state

        CalculatePatrolDirection();
        lineRenderer.enabled = true; // Show cone again
        UpdateLineRendererShape();
    }




    Vector3 GetDefaultForward()
    {
        // Might need to be changed depending on which direction the sprite faces
        return transform.right;
    }

    void RotateTowards(Vector3 direction, float speed)
    {
        if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // Use Slerp for smoother rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, targetAngle), Time.deltaTime * speed);
            currentForwardDirection = transform.right; // Or transform.up depending on your setup
        }
    }


    void SetupLineRenderer()
    {
        // Ensure counts and properties are set correctly
        int requiredPoints = coneResolution + 2; 
        if (lineRenderer.positionCount != requiredPoints) { lineRenderer.positionCount = requiredPoints; }

        lineRenderer.useWorldSpace = false; // Cone moves with the enemy
        lineRenderer.loop = true; // Try true for a closed shape

        lineRenderer.startWidth = coneLineWidth;
        lineRenderer.endWidth = coneLineWidth;
        lineRenderer.startColor = coneColor;
        lineRenderer.endColor = coneColor;

        if (lineMaterial != null) { lineRenderer.material = lineMaterial; }
        else if (lineRenderer.sharedMaterial == null) { Debug.LogWarning("LineRenderer material not set.", this); }
        else { lineRenderer.material = lineRenderer.sharedMaterial; } // Use shared if assigned in inspector
    }

    void UpdateLineRendererShape()
    {
        if (!lineRenderer.enabled || obstacleMask == 0) // Also check if obstacleMask is assigned
        {
            // If disabled or no obstacle mask is set, draw the simple cone
            DrawSimpleCone();
            return;
        }

        int pointsOnArc = coneResolution + 1;
        int totalPoints = pointsOnArc + 1; // Apex + Points on Arc
        if (lineRenderer.positionCount != totalPoints) { lineRenderer.positionCount = totalPoints; }

        // Set the apex of the cone (enemy's local origin)
        lineRenderer.SetPosition(0, Vector3.zero);

        // We need the world space forward direction for raycasting
        // Note: Since rotation happens in Update, using transform.right here is correct
        // if your sprite's "forward" is its local right. Adjust if needed (e.g., transform.up).
        Vector3 worldForward = transform.right; // Assuming sprite faces right

        for (int i = 0; i < pointsOnArc; i++)
        {
            // Calculate the angle for this point on the arc
            float fraction = (float)i / (pointsOnArc - 1);
            float currentAngle = viewAngle / 2.0f - (viewAngle * fraction);

            // Calculate the direction vector in world space based on the current angle
            // We rotate around the Z-axis for 2D
            Vector3 direction = Quaternion.AngleAxis(currentAngle, transform.forward) * worldForward; // Use transform.forward for Z-axis rotation

            // Raycast from the enemy's position in the calculated direction
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewDistance, obstacleMask);

            Vector3 pointPosition; // This will be in local space for the LineRenderer

            if (hit.collider != null)
            {
                // Hit an obstacle! Place the point at the hit location.
                // Convert the world hit point back to the enemy's local space.
                pointPosition = transform.InverseTransformPoint(hit.point);
            }
            else
            {
                // No obstacle hit. Place the point at the full viewDistance.
                // Convert the world direction to local space for positioning the point
                // (Since line renderer is in local space, we need local direction * distance)
                Vector3 localDirection = transform.InverseTransformDirection(direction);
                pointPosition = localDirection.normalized * viewDistance; // Use normalized local direction
            }

            // Set the position for this point on the arc (index i+1 because index 0 is the apex)
            lineRenderer.SetPosition(i + 1, pointPosition);
        }
    }

    // Helper function for the original cone drawing logic (used if obstacleMask is not set)
    void DrawSimpleCone()
    {
        if (!lineRenderer.enabled) return;

        int pointsOnArc = coneResolution + 1;
        int totalPoints = pointsOnArc + 1; // Apex + Points on Arc
        if (lineRenderer.positionCount != totalPoints) { lineRenderer.positionCount = totalPoints; }

        // Local forward depends on how your enemy is oriented, typically Vector3.right or Vector3.up
        Vector3 localForward = Vector3.right; // Match this with worldForward assumption in the main function

        lineRenderer.SetPosition(0, Vector3.zero); // Apex

        for (int i = 0; i < pointsOnArc; i++)
        {
            float fraction = (float)i / (pointsOnArc - 1);
            float currentAngle = viewAngle / 2.0f - (viewAngle * fraction);
            // Rotate around the local Z-axis (0,0,1) for 2D
            Vector3 direction = Quaternion.Euler(0, 0, currentAngle) * localForward;
            Vector3 arcPointPosition = direction * viewDistance;
            lineRenderer.SetPosition(i + 1, arcPointPosition);
        }
    }
}