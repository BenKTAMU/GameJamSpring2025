using UnityEngine;

public class ThrowManager : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject playerObj;
    public LineRenderer lineRenderer;
    public float powerMultiplier;
    public LayerMask wallLayerMask;
    private Vector3 dragStartPos;
    private Camera mainCam;
    public AudioSource throwSound;


    public float maxForce = 10f;
    
    
    
    //public LayerMask playerLayerMask;

    void Start()
    {
        mainCam = Camera.main;
        lineRenderer.sortingLayerName = "Foreground";
        lineRenderer.sortingOrder = 10;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 dragEndPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            Vector3 force = (dragStartPos - dragEndPos) * powerMultiplier;
            ShowTrajectory(playerObj.transform.position, force);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector3 dragEndPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            Vector3 force = (dragStartPos - dragEndPos) * powerMultiplier;
            FireProjectile(force);
            lineRenderer.positionCount = 0; // Clear line
        }
    }

    void ShowTrajectory(Vector3 startPos, Vector3 force)
    {
        float trajectoryLength = force.magnitude * 0.5f;
        Debug.Log("Trajectory legnth: " + trajectoryLength);
        lineRenderer.positionCount = Mathf.Clamp((int)trajectoryLength, 0, 500);
        //lineRenderer.positionCount = 500; // Adjust for trajectory detail
        Vector3 velocity = force;
        float timestep = 0.1f;
        Vector3 currentPos = startPos;
        int bounceLimit = 3; // Set the maximum number of bounces
        int bounceCount = 0;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, currentPos);

            if (bounceCount >= bounceLimit)
            {
                currentPos += velocity * timestep;
                continue;
            }

            RaycastHit2D[] hits = Physics2D.RaycastAll(currentPos, velocity, velocity.magnitude * timestep, wallLayerMask);
            if (hits.Length > 0)
            {
                RaycastHit2D hit = hits[0];
                Vector2 normal = hit.normal;
                velocity = Vector2.Reflect(velocity, normal);
                currentPos = hit.point + normal * 0.01f; // Move slightly away from the wall to avoid immediate re-collision
                bounceCount++;
            }
            else
            {
                currentPos += velocity * timestep;
            }
        }
    }
    void FireProjectile(Vector3 force)
    {
        if (force.magnitude > maxForce)
        {
            force = force.normalized * maxForce;
        }
        GameObject proj = Instantiate(projectilePrefab, playerObj.transform.position, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        throwSound.Play();
        
        if (rb != null)
        {
            rb.velocity = force;
        }
    }
}