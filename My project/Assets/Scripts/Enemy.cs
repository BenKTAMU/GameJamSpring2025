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
    Debug.Log(collision.gameObject.tag);
    if (collision.gameObject.tag == "Projectile")
    {
        if (collision.contacts.Length > 0)
        {
            Vector2 collisionDirection = collision.contacts[0].point - (Vector2)transform.position;
            Vector2 enemyForward = transform.right;

            if (Vector2.Dot(collisionDirection, enemyForward) < 0)
            {
                

                if (stateManager != null)
                {
                    stateManager.EnemyDecrement();
                }
                else
                {
                    Debug.LogWarning("StateManager is null!");
                }

                Destroy(collision.gameObject);
                Destroy(gameObject);
                Instantiate(particleSystem, transform.position, Quaternion.identity);
                enemyHitSound.Play();

                Debug.Log("Enemy hit from behind");
            }
            else
            {
                Debug.Log("Enemy not hit from behind");
            }
        }
        else
        {
            Debug.LogWarning("No collision contacts found!");
        }
    }
}
    
    
}
