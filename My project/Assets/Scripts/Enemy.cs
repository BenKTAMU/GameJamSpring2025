using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    private StateManager stateManager;
    public ParticleSystem particleSystem;
    public AudioSource enemyHitSound;
    void Start()
    {
        GameObject stateManagerObject = GameObject.Find("StateManager");
        if (stateManagerObject != null)
        {
            stateManager = stateManagerObject.GetComponent<StateManager>();
        }
        
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
                enemyHitSound.Play();
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
