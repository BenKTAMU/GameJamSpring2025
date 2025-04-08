using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;


    public Rigidbody2D rb;

    public GameObject projectilePrefab;

    public Camera cam;

    private Vector2 movement;
    private Vector2 mousePos;
    private bool isAttacking;

    public Transform firePoint;
    
    
    
    

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButtonDown(1))
        {
            meleeAttack();
        }
        
        
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
        
        
    }

    void meleeAttack()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rbProjectile = projectile.GetComponent<Rigidbody2D>();
        if (rbProjectile != null)
        {
            rbProjectile.AddForce(transform.up * 3f, ForceMode2D.Impulse);
        }
        Destroy(projectile, 0.2f);
        //StartCoroutine(ResetAttack());
    }
    
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    IEnumerator ResetAttack()
    {
        moveSpeed = 0.8f;
        yield return new WaitForSeconds(2f);
        isAttacking = false;
        moveSpeed = 5f;
    }
}
