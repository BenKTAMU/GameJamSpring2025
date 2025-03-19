using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int enemyCount;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyCount == 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void EnemyDecrement()
    {
        enemyCount--;
    }
    
    
}
