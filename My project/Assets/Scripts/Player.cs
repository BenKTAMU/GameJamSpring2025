using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;

    public GameObject projectilePrefab;
    public float maxDragDistance = 10f;
    public float projectileSpeedMultiplier = 3f;
    public LineRenderer lineRenderer;
    public int trajectoryResolution = 30;

    public AudioSource knifeThrow;





    private Vector2 movement;
    private Vector2 dragStartPos;
    private bool isDragging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the GameObject.");
        }

        if(lineRenderer == null)
        {
            Debug.LogError("LineRenderer component not found on the GameObject.");
        }
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        Debug.Log("Movement Input: " + movement);

        if(Input.GetMouseButtonDown(0))
        {
            dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }


        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 dragCurrentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = dragStartPos - dragCurrentPos;
            float dragDistance = Mathf.Clamp(dragVector.magnitude, 0, maxDragDistance);
            Vector2 launchDirection = dragVector.normalized;

            UpdateTrajectory(launchDirection, dragDistance * projectileSpeedMultiplier);
        }


        if(Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = dragStartPos - dragEndPos;
            float dragDistance = Mathf.Clamp(dragVector.magnitude, 0, maxDragDistance);
            Vector2 launchDirection = dragVector.normalized;

            FireProjectile(launchDirection, dragDistance * projectileSpeedMultiplier);
            isDragging = false;
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
            Debug.Log("Player Position: " + rb.position);
        }
    }

    void FireProjectile(UnityEngine.Vector2 direction, float speed)
    {   
        knifeThrow.Play();
        GameObject projectile = Instantiate(projectilePrefab, transform.position, UnityEngine.Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if(projectile != null)
        {
            rb.velocity = direction * speed;
        }
        lineRenderer.positionCount = 0;
        
    }

    void UpdateTrajectory(Vector2 direction, float speed)
    {
        Vector2[] trajectoryPoints = CalculateTrajectory(transform.position, direction, speed, trajectoryResolution);
        lineRenderer.positionCount = trajectoryPoints.Length;
        for (int i = 0; i < trajectoryPoints.Length; i++)
        {
            lineRenderer.SetPosition(i, trajectoryPoints[i]);
        }
    }

    Vector2[] CalculateTrajectory(Vector2 startPosition, Vector2 direction, float speed, int resolution)
    {
        Vector2[] points = new Vector2[resolution];
        float timeStep = 0.1f;
        Vector2 velocity = direction * speed;
        Vector2 currentPosition = startPosition;

        for (int i = 0; i < resolution; i++)
        {
            points[i] = currentPosition;
            Vector2 nextPosition = currentPosition + velocity * timeStep;

            RaycastHit2D hit = Physics2D.Raycast(currentPosition, velocity, (nextPosition - currentPosition).magnitude);
            if (hit.collider != null)
            {
                points[i] = hit.point;
                Vector2 reflectDirection = Vector2.Reflect(velocity.normalized, hit.normal);
                velocity = reflectDirection * velocity.magnitude; // Maintain the same speed after reflection
                currentPosition = hit.point;
            }
            else
            {
                currentPosition = nextPosition;
            }
        }
        return points;
    }
}