using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float bounciness = 0.8f;
    public float lifetime;
    public AudioClip wallHitSound; 
    private Rigidbody2D rb;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GameObject.Find("KnifeBounce").GetComponent<AudioSource>();
        wallHitSound = audioSource.clip;
        if (audioSource == null)
        {
            
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall")) // Check if the object hit is tagged as "Wall"
        {
            if (wallHitSound != null)
            {
                audioSource.PlayOneShot(wallHitSound); // Play the sound
            }
            else
            {
                Debug.Log("no");
            }
        }
    }
}