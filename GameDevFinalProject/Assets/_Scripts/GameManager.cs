using System;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager instance;

    // UI References
    public Text scoreText;
    public Text timerText;
    public Text livesText;
    public Text levelText;
    public Text finalScoreText;
    public InputField feedbackInputField;
    public GameObject endMenu;
    public GameObject pauseMenu;
    public Slider timerSlider;
    public GameObject levelCompleteCanvas;
    public GameObject startLevelCanvas;

    // Game Variables
    public GameObject balloonsParent;
    public Transform playerStartPosition;
    public int totalLives = 3;
    public float sessionDuration = 240f;

    private int score = 0;
    private int lives;
    private float remainingTime;
    private bool isPaused = false;

    private int totalBalloons = 0;
    private int balloonsPopped = 0;

    public string[] levels;
    private int currentLevelIndex = 0;

    private bool isPaused = false;

    public GameObject player;
    private FollowMouse followMouseScript;

    public GameObject endMenuCanvas;
    public GameObject pauseMenuCanvas;
    public GameObject levelCompleteCanvas;
    public GameObject startLevelCanvas;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (GameData.CurrentLevel == 0)
        {
            GameData.Reset(); // Initialize data for a new game
        }

        score = GameData.Score;
        lives = GameData.Lives > 0 ? GameData.Lives : totalLives;
        remainingTime = GameData.RemainingTime > 0 ? GameData.RemainingTime : sessionDuration;

        InitializeGame();
    }

    void InitializeGame()
    {
        sessionEnded = false;

        uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager instance is not available!");
            return;
        }

        balloonsParent = GameObject.Find("BalloonsParent");
        if (balloonsParent == null)
        {
            Debug.LogError("BalloonsParent not found in the scene!");
            return;
        }

        totalBalloons = balloonsParent.transform.childCount;
        balloonsPopped = 0;

        if (player != null)
        {
            followMouseScript = player.GetComponent<FollowMouse>();
        }

        if (balloonsParent != null && totalBalloons == 0)
        {
            totalBalloons = balloonsParent.transform.childCount; // Set balloon count only if it's not set already
        }
        else
        {
            Debug.LogWarning("BalloonsParent not assigned in GameManager!");
        }

        ResetPlayerPosition();
        UpdateUI();

        Time.timeScale = 1f;

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (timerSlider != null)
        {
            timerSlider.maxValue = sessionDuration;
            timerSlider.value = remainingTime;
        }
    }


    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);

                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            if (timerSlider != null)
            {
                timerSlider.value = remainingTime;
            }
        }
        else
        {
            EndSession();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        Time.timeScale = 1f;

        balloonsParent = GameObject.Find("BalloonsParent");
        if (balloonsParent == null)
        {
            Debug.LogError("BalloonsParent not found in the scene!");
        }
        else
        {
            totalBalloons = balloonsParent.transform.childCount;
            balloonsPopped = 0; // Reset popped balloons count
        }

        uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            uiManager.Initialize();
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("Start Level Canvas is not assigned!");
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseMenu.SetActive(isPaused);
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        // Pause the game: disable movement, etc., but keep UI responsive.
        Time.timeScale = 0f;
        player.GetComponent<FollowMouse>().enabled = false; // Disable player movement
    }

    void ResumeGame()
    {
        // Resume the game: re-enable player movement and resume normal time scale
        Time.timeScale = 1f;
        player.GetComponent<FollowMouse>().enabled = true; // Enable player movement
    }

    public void PlayerHitObstacle()
    {
        lives--;

        if (lives > 0)
        {
            Debug.Log($"Player hit an obstacle! Lives remaining: {lives}");
            ResetPlayerPosition();
        }
        else
        {
            Debug.Log("Game Over! Lives exhausted.");
            EndSession();
        }

        UpdateUI();
    }

    public void BalloonPopped()
    {
        balloonsPopped++;
        score++;
        GameData.Score = score; // Synchronize with GameData
        UpdateUI();

        if (balloonsPopped == totalBalloons)
        {
            Debug.Log("All balloons popped! Loading next level...");
            ShowLevelCompleteCanvas();
        }
    }


    void ShowLevelCompleteCanvas()
    {
        levelCompleteCanvas.SetActive(true);
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

    }

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

    public void LoadNextLevel()
    {
        // Save current state
        GameData.Score = score;
        GameData.Lives = lives;
        GameData.RemainingTime = remainingTime;

        currentLevelIndex++;
        GameData.CurrentLevel = currentLevelIndex;

        if (currentLevelIndex < levels.Length)
        {
            string nextSceneName = levels[currentLevelIndex];
            Debug.Log($"Loading next level: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("No more levels to load. Ending session.");
            EndSession();
        }
    }

    public void StartLevel()
    {
        startLevelCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    void EndSession()
    {
        Debug.Log("Session ended.");

        // Save game data
        GameData.Score = score;
        GameData.Lives = 0;
        GameData.RemainingTime = 0;

        if (uiManager != null)
        {
            finalScoreText.text = "Here is your final score: " + score;
        }

        SaveSessionData();
        endMenuCanvas?.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SaveSessionData()
    {
        string feedback = feedbackInputField?.text ?? "No feedback provided";

        SessionData data = new SessionData
        {
            string filePath = Path.Combine(Application.persistentDataPath, "SessionData.json");
            File.WriteAllText(filePath, JsonUtility.ToJson(new SessionData
            {
                PlayerID = Guid.NewGuid().ToString(),
                StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Duration = (sessionDuration - remainingTime).ToString("F2") + " seconds",
                Score = score
            }, true));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save session data: {ex.Message}");
        }
    }

    public void RestartGame()
    {
        if (uiManager == null)
        {
            Debug.LogError("UIManager is not available!");
            return;
        }

        uiManager.UpdateScore(GameData.Score);
        uiManager.UpdateLives(GameData.Lives); // Ensure UI reflects the updated lives
        uiManager.UpdateTimer(GameData.RemainingTime, sessionDuration);
        uiManager.UpdateLevelNumber(GameData.CurrentLevel + 1);
    }

    void ShowLevelCompleteCanvas()
    {
        if (levelCompleteCanvas != null)
        {
            levelCompleteCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("Level complete canvas is not assigned!");
        }
    }

    public void QuitGame()
    {
        lives--; // Decrement lives
        GameData.Lives = lives; // Synchronize with GameData

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        UpdateUI();
    }

    private IEnumerator ShowStartLevelCanvasWithDelay()
    {
        yield return new WaitForSeconds(1.5f);
        ShowStartLevelCanvas();
    }

    void ShowStartLevelCanvas()
    {
        if (startLevelCanvas != null)
        {
            startLevelCanvas.SetActive(true);
            Time.timeScale = 0f; // Pause temporarily
        }
    }

    public void StartLevel()
    {
        if (startLevelCanvas != null)
        {
            startLevelCanvas.SetActive(false);
        }

        Time.timeScale = 1f; // Resume gameplay
        if (followMouseScript != null)
        {
            followMouseScript.enabled = true; // Re-enable player movement
        }
    }

    void ResetPlayerPosition()
    {
        if (player != null && playerStartPosition != null)
        {
            player.transform.position = playerStartPosition.position;
        }
    }

    [Serializable]
    public class SessionData
    {
        public string PlayerID;
        public string StartTime;
        public string Duration;
        public int Score;
    }
}
