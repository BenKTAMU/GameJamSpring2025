using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int enemyCount;
    public int level;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyCount == 0)
        {
            if (level == 1)
            {
                SceneManager.LoadScene("Level 2");
            }
            else if(level == 2)
            {
                SceneManager.LoadScene("Level 3");
            }
        }
    }

    public void EnemyDecrement()
    {
        enemyCount--;
    }
    
    
}
