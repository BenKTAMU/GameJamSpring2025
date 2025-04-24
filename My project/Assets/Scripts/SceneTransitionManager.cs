using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public Image fadePanel; // Reference to the black panel
    public float fadeDuration = 1f;

    private void Start()
    {
        // Start with a fade-in effect
        StartCoroutine(FadeIn());
    }

    public void ReloadScene()
    {
        StartCoroutine(FadeOutAndReload());
    }

    private IEnumerator FadeIn()
    {
        //float elapsedTime = 0f;
        Color color = fadePanel.color;



        color.a = 0f;
        fadePanel.color = color;
        yield return null;
    }

    private IEnumerator FadeOutAndReload()
    {
        float elapsedTime = 0f;
        Color color = fadePanel.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = 1f;
        fadePanel.color = color;

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}