using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{   

    public AudioSource hurt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Projectile")
        {
            Debug.Log("Enemy hit by a projectile.");
            Vector2 collisionDirection = collision.relativeVelocity.normalized;
            Vector2 enemyForward = transform.up;

            if(Vector2.Dot(collisionDirection, enemyForward) < 0)
            {
                Debug.Log("Enemy hit from the back.");
                hurt.Play();
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Enemy hit from the front.");
            }
        }
    }
}
