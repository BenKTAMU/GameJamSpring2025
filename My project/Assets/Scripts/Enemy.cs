using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private StateManager stateManager;
    public ParticleSystem particleSystem;
    public AudioSource enemyHitSound;
    private GameObject stateManagerObject;
    private bool isDestroyed = false; // Flag to prevent multiple calls

    void Awake()
    {
        stateManagerObject = GameObject.Find("StateManager");
        if (stateManagerObject != null)
        {
            stateManager = stateManagerObject.GetComponent<StateManager>();
            if (stateManager == null)
            {
                Debug.LogWarning("StateManager component not found on the StateManager GameObject!");
            }
        }
        else
        {
            Debug.LogWarning("StateManager GameObject not found!");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDestroyed) return; // Prevent further execution if already destroyed

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

                    isDestroyed = true; // Set the flag to true
                    Destroy(gameObject);
                    Destroy(collision.gameObject);
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