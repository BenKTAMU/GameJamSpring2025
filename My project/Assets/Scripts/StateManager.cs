using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int enemyCount;
    public int level;
    public Text enemyCountText;
    void Start()
    {
        UpdateEnemyCountText();
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
        Debug.Log("EnemyDecrement called. Current enemy count: " + enemyCount);
        UpdateEnemyCountText();
        
    }

    public void playerDeath()
    {
        FindObjectOfType<SceneTransitionManager>().ReloadScene();
    }

    private void UpdateEnemyCountText()
    {
        if(enemyCountText != null)
        {
            enemyCountText.text = "Enemies Left: " + enemyCount;
        }
        else
        {
            Debug.LogError("Enemy Count Text is not assigned in the inspector.");
        }
    }
    
    
}
