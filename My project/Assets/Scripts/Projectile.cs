using UnityEngine;


public class Projectile : MonoBehaviour
{
    public float bounciness = 0.8f;
    public float lifetime;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }


    
    
}