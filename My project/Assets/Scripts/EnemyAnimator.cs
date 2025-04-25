using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    public GameObject parent;
    public float parent_xrot;
    public float parent_yrot;
    public float parent_zrot;
    // Start is called before the first frame update
    public float xDirection;
    public float yDirection;
    public float lastYDirection = 0;
    public float lastXDirection = 0;
    public float velocityX;
    public float velocityY;
    public float lastVelocityX = 0;
    public float lastVelocityY = 0;
    public Animator animator;
    public int sillyNumber;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (sillyNumber == 1)
        {
            parent_xrot = parent.transform.rotation.x;
            parent_yrot = parent.transform.rotation.y;
            parent_zrot = parent.transform.rotation.z;
            gameObject.transform.rotation = Quaternion.Euler(-parent_xrot, - parent_yrot, - parent_zrot);
            animator.SetFloat("IdleX", 1);
            animator.SetFloat("IdleY", 0);
        }
        else
        {
            parent_xrot = parent.transform.rotation.x;
            parent_yrot = parent.transform.rotation.y;
            parent_zrot = parent.transform.rotation.z;
            gameObject.transform.rotation = Quaternion.Euler(-parent_xrot, - parent_yrot, - parent_zrot);
        
            xDirection = parent.transform.position.x;
            yDirection = parent.transform.position.y;
            velocityX = (xDirection - lastXDirection)/Time.deltaTime;
            velocityY = (yDirection-lastYDirection)/Time.deltaTime;

            if(velocityX != 0 && velocityY != 0 || lastVelocityX != 0 && lastVelocityY != 0)
            {
                animator.SetBool("isMoving", true);
                animator.SetFloat("moveX", velocityX);
                animator.SetFloat("moveY", velocityY);
            }
            else
            {
                animator.SetBool("isMoving", false);
                animator.SetFloat("IdleX", lastVelocityX);
                animator.SetFloat("IdleY", lastVelocityY);
            }

            lastXDirection = xDirection;
            lastYDirection = yDirection;
            lastVelocityX = velocityX;
            lastVelocityY = velocityY;
        }
        
    }
}
