using UnityEngine;

public class StartScreenManager : MonoBehaviour
{
    public GameObject startScreenCanvas; // Reference to the start screen canvas
    public GameObject player;           // Reference to the player object

    private FollowMouse followMouseScript; // Reference to the FollowMouse component

    void Start()
    {
        if (player != null)
        {
            followMouseScript = player.GetComponent<FollowMouse>();
            if (followMouseScript != null)
            {
                followMouseScript.enabled = false; // Disable player movement
            }
        }

        startScreenCanvas.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void StartGame()
    {
        startScreenCanvas.SetActive(false);
        Time.timeScale = 1f; // Resume the game

        if (followMouseScript != null)
        {
            followMouseScript.enabled = true; // Enable player movement
        }
    }

    public void ShowStartScreen()
    {
        startScreenCanvas.SetActive(true);

        if (followMouseScript != null)
        {
            followMouseScript.enabled = false; // Ensure movement is disabled
        }

        Time.timeScale = 0f;
    }
}
