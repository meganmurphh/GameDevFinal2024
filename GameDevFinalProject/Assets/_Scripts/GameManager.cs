using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // UI References
    public Text scoreText;
    public Text timerText;
    public Text livesText;
    public Text levelText;
    public InputField feedbackInputField;

    public GameObject pauseMenu;
    public GameObject endMenu;

    // Game Variables
    public GameObject balloonsParent; // Parent containing all balloons
    public GameObject player;         // Reference to the player object
    public Transform playerStartPosition; // Starting position of the player

    public int totalLives = 3;        // Total player lives
    public float sessionDuration = 240f; // 4 minutes

    private int score = 0;            // Player's score
    private int lives;                // Current lives
    private int totalBalloons = 0;    // Total number of balloons in the level
    private int balloonsPopped = 0;   // Balloons popped
    private float remainingTime;      // Remaining session time
    private bool isPaused = false;    // Pause state

    private DateTime sessionStartTime; // Time when the session started
    private string playerID = "Player_" + Guid.NewGuid(); // Unique player ID

    // Level Management
    public string[] levels;           // Array of level scene names
    private int currentLevelIndex = 0;

    void Start()
    {
        // Initialize variables
        lives = totalLives;
        remainingTime = sessionDuration;
        sessionStartTime = DateTime.Now;

        // Validate the levels array
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("Levels array is not initialized! Please assign scene names in the Inspector.");
            return;
        }

        // Get the current level index based on the active scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        currentLevelIndex = Array.IndexOf(levels, currentSceneName);

        if (currentLevelIndex == -1)
        {
            Debug.LogError($"Current scene '{currentSceneName}' is not in the levels array!");
            return;
        }

        // Count the balloons at the start
        if (balloonsParent != null)
        {
            totalBalloons = balloonsParent.transform.childCount;
        }
        else
        {
            Debug.LogWarning("BalloonsParent not assigned in GameManager!");
        }

        // Reset the player position
        ResetPlayerPosition();

        // Update UI
        UpdateUI();

        // Ensure game is running
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Update timer
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = "Time: " + Mathf.CeilToInt(remainingTime) + "s";
            }
        }
        else
        {
            EndSession();
        }
    }

    // Called when the player collides with a red zone or bomb
    public void PlayerHitObstacle()
    {
        lives--; // Decrease lives

        if (lives > 0)
        {
            Debug.Log($"Player hit an obstacle! Lives remaining: {lives}");
            ResetPlayerPosition(); // Restart the player
        }
        else
        {
            Debug.Log("Game Over! Lives exhausted.");
            EndSession(); // End the game if lives are 0
        }

        UpdateUI(); // Update the UI to reflect changes in lives
    }

    // Called by PopBalloon script when a balloon is popped
    public void BalloonPopped()
    {
        balloonsPopped++;
        score++;
        UpdateUI();

        Debug.Log($"Balloon popped! {balloonsPopped}/{totalBalloons}");

        if (balloonsPopped == totalBalloons)
        {
            Debug.Log("All balloons popped! Loading next level...");
            LoadNextLevel();
        }
    }

    // Resets the player's position to the starting point
    void ResetPlayerPosition()
    {
        if (player != null && playerStartPosition != null)
        {
            player.transform.position = playerStartPosition.position;
        }
        else
        {
            Debug.LogWarning("Player or PlayerStartPosition not set in GameManager!");
        }
    }

    // Loads the next level using the levels array
    void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levels.Length)
        {
            string nextSceneName = levels[currentLevelIndex];
            Debug.Log($"Loading next level: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName); // Load the next scene by name
        }
        else
        {
            Debug.Log("No more levels to load. Ending session.");
            EndSession();
        }
    }

    // Ends the session
    void EndSession()
    {
        Debug.Log("Session ended.");
        SaveSessionData();
        Time.timeScale = 0f;
        endMenu.SetActive(true);
    }

    // Saves session data locally
    void SaveSessionData()
    {
        string feedback = feedbackInputField?.text ?? "No feedback provided";

        SessionData data = new SessionData
        {
            PlayerID = playerID,
            StartTime = sessionStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
            Duration = (sessionDuration - remainingTime).ToString("F2") + " seconds",
            Score = score,
            Feedback = feedback
        };

        string filePath = Path.Combine(Application.persistentDataPath, "SessionData.json");
        File.WriteAllText(filePath, JsonUtility.ToJson(data, true));
        Debug.Log("Session data saved to: " + filePath);
    }

    // Pauses or resumes the game
    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    // Restarts the game
    public void RestartGame()
    {
        SceneManager.LoadScene(levels[0]); // Restart from the first level
    }

    // Quits the game
    public void QuitGame()
    {
        Application.Quit();
    }

    // Updates the UI elements
    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }

        if (levelText != null)
        {
            levelText.text = $"Level: {currentLevelIndex + 1}";
        }
    }
}

// Serializable structure for session data
[Serializable]
public class SessionData
{
    public string PlayerID;
    public string StartTime;
    public string Duration;
    public int Score;
    public string Feedback;
}
