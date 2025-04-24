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
    private Vector2 animMovement;
    private Vector2 mousePos;
    private bool isAttacking;

    public Transform firePoint;
    
    public Animator animator;

    public StateManager stateManager;
    
    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isStabbing", false);
        // Prioritize horizontal movement
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = 0; // Disable vertical movement
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            movement.y = Input.GetAxisRaw("Vertical");
            movement.x = 0; // Disable horizontal movement
        }
        else
        {
            movement.x = 0;
            movement.y = 0;
        }

        animMovement.x = movement.x;
        animMovement.y = movement.y;

        animator.SetFloat("moveX", animMovement.x);
        animator.SetFloat("moveY", animMovement.y);

        if (animMovement.x != 0 || animMovement.y != 0)
        {
            animator.SetBool("isMoving", true);
            Animate();
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("isStabbing", true);
            meleeAttack();

        }

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        //
        // Vector2 lookDir = mousePos - rb.position;
        // float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        // rb.rotation = angle;
        
        
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
            stateManager.playerDeath();
        }
    }

    IEnumerator ResetAttack()
    {
        moveSpeed = 0.8f;
        yield return new WaitForSeconds(2f);
        isAttacking = false;
        moveSpeed = 5f;
    }

    private void Animate()
    {
        if(Input.GetAxis("Horizontal")!= 0 || Input.GetAxis("Vertical") != 0)
        {
            animator.SetFloat("LastMoveX",Input.GetAxis("Horizontal"));
            animator.SetFloat("LastMoveY", Input.GetAxis("Vertical"));
        }
    }
}
