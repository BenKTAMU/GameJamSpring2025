using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StateManager : MonoBehaviour
{
    public int enemyCount;
    public int level;
    public Text enemyCountText;
    public AudioSource backgroundMusic;
    public AudioSource levelClearMusic;

    private bool isLevelClearing = false; 

    void Start()
    {
        UpdateEnemyCountText();
    }

    void FixedUpdate() 
    {
 
        if (enemyCount == 0 && !isLevelClearing)
        {
            isLevelClearing = true; 
            if (level == 1)
            {
                Debug.Log("Level 1 Clear Triggered!");
                if (backgroundMusic != null && backgroundMusic.isPlaying)
                {
                    backgroundMusic.Stop();
                }
                if (levelClearMusic != null && levelClearMusic.clip != null)
                {
                    levelClearMusic.PlayOneShot(levelClearMusic.clip);
                    StartCoroutine(PlaySoundAndLoadScene(levelClearMusic, "Level 2"));
                }
                else
                {
                    Debug.LogError("Level Clear Music or Clip is missing for Level 1!");
                    SceneManager.LoadScene("Level 2");
                }
            }
            else if (level == 2)
            {
                 Debug.Log("Level 2 Clear Triggered!"); 

                 if (backgroundMusic != null && backgroundMusic.isPlaying)
                 {
                    backgroundMusic.Stop(); 
                 }


                 if (levelClearMusic != null && levelClearMusic.clip != null)
                 {
                     levelClearMusic.PlayOneShot(levelClearMusic.clip); // Play the sound
                     StartCoroutine(PlaySoundAndLoadScene(levelClearMusic, "Level 3"));
                 }
                 else
                 {
                     Debug.LogError("Level Clear Music or Clip is missing for Level 2!");

                 }
            }
            else if (level == 3)
            {
                Debug.Log("Level 2 Clear Triggered!"); 

                if (backgroundMusic != null && backgroundMusic.isPlaying)
                {
                    backgroundMusic.Stop(); 
                }


                if (levelClearMusic != null && levelClearMusic.clip != null)
                {
                    levelClearMusic.PlayOneShot(levelClearMusic.clip); // Play the sound
                    StartCoroutine(PlaySoundAndLoadScene(levelClearMusic, "Game Over"));
                }
                else
                {
                    Debug.LogError("Level Clear Music or Clip is missing for Level 2!");

                }
            }

        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            Application.Quit();
        }
    }

    private IEnumerator PlaySoundAndLoadScene(AudioSource audioSource, string sceneName)
    {

        if (audioSource != null && audioSource.clip != null)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        else
        {

             Debug.LogWarning("AudioSource or clip missing in PlaySoundAndLoadScene, loading scene immediately.");

        }
        SceneManager.LoadScene(sceneName);
    }

    public void EnemyDecrement()
    {
        if (enemyCount > 0) 
        {
             enemyCount--;
             Debug.Log("EnemyDecrement called. Current enemy count: " + enemyCount);
             UpdateEnemyCountText();
        }
    }

    public void playerDeath()
    {

        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
             backgroundMusic.Stop();
        }

        SceneTransitionManager transitionManager = FindObjectOfType<SceneTransitionManager>();
        if (transitionManager != null)
        {
            transitionManager.ReloadScene();
        }
        else
        {
            Debug.LogError("SceneTransitionManager not found in the scene!");

        }
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
