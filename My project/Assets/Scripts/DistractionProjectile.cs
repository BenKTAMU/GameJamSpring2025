using UnityEngine;

public class DistractionProjectile : MonoBehaviour
{
    public float distractionRadius;
    public LayerMask enemyLayer;        
    public LayerMask wallLayer;   

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        // Check if we hit a wall using the wallLayer mask
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            Debug.Log("Projectile hit a wall: " + collision.collider.name);

            // Get the point of impact
            Vector3 impactPoint = collision.contacts[0].point; // Use the first contact point

            // Find all colliders within the distractionRadius on the enemyLayer
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(impactPoint, distractionRadius, enemyLayer);

            foreach (var hitCollider in hitColliders)
            {
                // Try to get the EnemyPatrolManual script from the detected enemy
                EnemyPatrolManual enemy = hitCollider.GetComponent<EnemyPatrolManual>();
                if (enemy != null)
                {
                    // Tell the enemy about the distraction
                    enemy.HearDistraction(impactPoint);
                }
            }
            
        }
    }
}