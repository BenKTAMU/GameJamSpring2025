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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        parent_xrot = parent.transform.rotation.x;
        parent_yrot = parent.transform.rotation.y;
        parent_zrot = parent.transform.rotation.z;
        gameObject.transform.rotation = Quaternion.Euler(-parent_xrot, - parent_yrot, - parent_zrot);
        

    }
}
