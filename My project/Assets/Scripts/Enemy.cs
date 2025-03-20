using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    private StateManager stateManager;
    public ParticleSystem particleSystem;
    void Start()
    {
        GameObject stateManagerObject = GameObject.Find("StateManager");
        if (stateManagerObject != null)
        {
            stateManager = stateManagerObject.GetComponent<StateManager>();
        }
        
        // var main = particleSystem.main;
        // main.startColor = Color.red; // Set particle color to red
        // main.startLifetime = 0.5f; // Set particle lifetime
        // main.startSpeed = 5f; // Set particle speed
        //
        // var shape = particleSystem.shape;
        // shape.shapeType = ParticleSystemShapeType.Sphere; // Set shape to sphere
        //
        // var emission = particleSystem.emission;
        // emission.rateOverTime = 0; // Disable continuous emission
        // emission.SetBursts(new ParticleSystem.Burst[] {
        //     new ParticleSystem.Burst(0f, 100) // Emit 100 particles instantly
        // });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            Vector2 collisionDirection = collision.contacts[0].point - (Vector2)transform.position;
            Vector2 enemyForward = -transform.up;

            if (Vector2.Dot(collisionDirection, enemyForward) < 0)
            {
                Instantiate(particleSystem, transform.position, Quaternion.identity);
                Destroy(gameObject);
                stateManager.EnemyDecrement();
                Debug.Log("Enemy hit from behind");
                
            }
            else
            {
                Debug.Log("Enemy not hit from behind");
            }
        }
    }
    
    
}
