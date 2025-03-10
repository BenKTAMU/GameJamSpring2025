using UnityEngine;


public class Projectile : MonoBehaviour
{
    public float bounciness = 0.8f;
    public float lifetime = 2f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        Vector2 incomingVelocity = rb.velocity;
        Vector2 reflectedVelocity = Vector2.Reflect(incomingVelocity, normal) * bounciness;
        rb.velocity = reflectedVelocity;
    }
}