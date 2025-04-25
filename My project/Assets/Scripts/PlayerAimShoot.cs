using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAimShoot : MonoBehaviour
{

    [Header("Aiming and Trajectory")] 
    public LineRenderer trajectoryLine;
    public int maxTrajectoryPoints = 25;
    public int maxBounces = 5;
    public LayerMask trajectoryCollisionMask;

    [Header("Projectile")] 
    public GameObject projectilePrefab;
    public Transform launchPoint;
    public float projectileSpeed = 15f;

    private Camera mainCamera;
    private Vector2 aimDirection;
    private Quaternion knifeRotation;
    private Vector2 aimDirectionNormalized;
    
    public Animator animator;
    public AudioSource knifeThrowAudio;
    
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        if (trajectoryLine == null)
        {
            
            Debug.LogError("Trajectory Line Renderer not assigned!");
            enabled = false;
        }

        if (launchPoint == null)
        {
            launchPoint = transform;
            Debug.LogWarning("Launch point not assigned, using player position.");
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab not assigned!");
            enabled = false;
        }

        trajectoryLine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleShooting();
    }

    void HandleShooting()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane + 10f)); //update according to where camera is
        mouseWorldPosition.z = launchPoint.position.z;

        aimDirection = (mouseWorldPosition - launchPoint.position).normalized;
        Quaternion knifeRotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Acos(aimDirection.x));

        if (Input.GetMouseButton(0))
        {
            animator.SetBool("throwReady", true);
            ShowTrajectory(launchPoint.position, aimDirection * projectileSpeed);
            trajectoryLine.enabled = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("throwReady", false);
            animator.SetBool("waterbucketRelease", true);
            
            if (GameObject.FindGameObjectsWithTag("Projectile").Length == 0)
            {
                FireProjectile(knifeRotation);
            }
            trajectoryLine.enabled = false;
        }
        else
        {
            trajectoryLine.enabled = false;
            animator.SetBool("throwReady", false);
        }
    }

    void ShowTrajectory(Vector2 startPoint, Vector2 initialVelocity)
    {
       // Debug.Log("Showing trajectory from: " + startPoint + " with velocity: " + initialVelocity);
        List<Vector3> points = new List<Vector3>();
        points.Add(startPoint);

        Vector2 currentPosition = startPoint;
        Vector2 currentVelocity = initialVelocity;
        int bounces = 0;

        for (int i = 1; i < maxTrajectoryPoints; i++)
        {
            float timeStep = 0.1f;
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, currentVelocity.normalized, currentVelocity.magnitude * timeStep, trajectoryCollisionMask);

            if (hit.collider != null)
            {
                points.Add(hit.point);

                if (bounces >= maxBounces)
                {
                    break;
                }
                
                currentVelocity = Vector2.Reflect(currentVelocity, hit.normal);
                currentPosition = hit.point + hit.normal * 0.1f; // Move slightly away from the wall to avoid immediate re-collision
                bounces++;
            }
            else
            {
                currentPosition += currentVelocity * timeStep;
                points.Add(currentPosition);
            }

            if (currentVelocity.sqrMagnitude < 0.01f) break;
        }
        
        trajectoryLine.positionCount = points.Count;
        trajectoryLine.SetPositions(points.ToArray());
    }

    void FireProjectile(Quaternion rotation)
    {
        if (projectilePrefab == null) return;
        
        GameObject projectileInstance = Instantiate(projectilePrefab, launchPoint.position, rotation);
        Debug.Log(rotation);
        Rigidbody2D rb = projectileInstance.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = aimDirection * projectileSpeed;
            knifeThrowAudio.Play();
            animator.SetBool("waterbucketRelease", false);
        }
        else
        {
            Debug.LogError("Projectile missing Rigidbody2D component!");
        }
    }
}
