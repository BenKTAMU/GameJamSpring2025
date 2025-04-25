using UnityEngine;
using UnityEngine.UI; // Required for Button interaction if not using TextMeshPro Button directly

public class Tutorial : MonoBehaviour
{
    // Drag your Tutorial Panel GameObject here in the Inspector
    public GameObject tutorialPanel;

    // Optional: Set this to true if the tutorial should pause the game
    public bool pauseGameWhenOpen = true;

    // Optional: To prevent the tutorial from showing again (e.g., after first close)
    private bool hasBeenShown = false;
    // Use static if you want it to persist across scene loads (simple way)
    // private static bool staticHasBeenShown = false;


    void Start()
    {
        // --- Choose ONE way to show the tutorial initially ---

        // Option A: Show it immediately when the scene/game starts
        // Make sure the TutorialPanel is ACTIVE in the Hierarchy by default if using this.
        // OR Ensure it's inactive and call ShowTutorial() 
        //if (!staticHasBeenShown) // Example using static flag
        //{
             ShowTutorial(); // Or just ensure it's active in the editor
        //} else {
        //   tutorialPanel.SetActive(false); // Ensure it's hidden if already shown
        //}


        // Option B: Keep the panel INACTIVE in the Hierarchy by default.
        // Call ShowTutorial() from another script when needed (e.g., OnTriggerEnter2D)
        // Example: FindObjectOfType<TutorialManager>().ShowTutorial();

        // For this example, let's assume it starts active or we call ShowTutorial() here.
        // If the panel starts active in the editor, this call might pause the game immediately.
        // ShowTutorial(); // Uncomment if you want it to show via script on Start

        // --- Initial setup based on editor state ---
        if (tutorialPanel.activeSelf) {
            // If it starts active, apply pause setting
             if (pauseGameWhenOpen)
            {
                Time.timeScale = 0f; // Pause game
            }
        }
    }

    // Call this method to display the tutorial
    public void ShowTutorial()
    {
        if (tutorialPanel != null /*&& !hasBeenShown*/) // Add !hasBeenShown if you only want it once
        {
            tutorialPanel.SetActive(true);
            if (pauseGameWhenOpen)
            {
                Time.timeScale = 0f; // Pause game time
            }
             // Optional: Set the flag so it doesn't show again
            // hasBeenShown = true;
            // staticHasBeenShown = true; // If using static
        }
    }

    // This method will be linked to the Button's OnClick event
    public void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
            if (pauseGameWhenOpen)
            {
                Time.timeScale = 1f; // Resume game time
            }
        }
    }

    // Optional: Allow closing with a key (e.g., Escape)
    void Update()
    {
        // Only check for input if the panel is currently active
        if (tutorialPanel != null && tutorialPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)) // Example: Escape or Enter
            {
                CloseTutorial();
            }
        }
    }
}