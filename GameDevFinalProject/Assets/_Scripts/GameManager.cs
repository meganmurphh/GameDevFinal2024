using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
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
    public GameObject player;
    public Transform playerStartPosition;

    public int totalLives = 3;
    public float sessionDuration = 240f;

    private int score = 0;
    private int lives;
    private int totalBalloons = 0;
    private int balloonsPopped = 0;
    private float remainingTime;
    private bool isPaused = false;

    private DateTime sessionStartTime;
    private string playerID = "Player_" + Guid.NewGuid();

    public string[] levels;
    private int currentLevelIndex = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        lives = totalLives;
        remainingTime = sessionDuration;
        sessionStartTime = DateTime.Now;

        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("Levels array is not initialized! Please assign scene names in the Inspector.");
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        currentLevelIndex = Array.IndexOf(levels, currentSceneName);


        if (currentLevelIndex == -1)
        {
            Debug.LogError($"Current scene '{currentSceneName}' is not in the levels array!");
            return;
        }

        if (balloonsParent != null)
        {
            totalBalloons = balloonsParent.transform.childCount;
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
        ReassignUIElements();
    }

    void ReassignUIElements()
    {
        scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();
        Debug.Log(scoreText != null ? "ScoreText found!" : "ScoreText missing!");

        scoreText = GameObject.Find("Score")?.GetComponent<Text>();
        timerText = GameObject.Find("Timer")?.GetComponent<Text>();
        livesText = GameObject.Find("Lives")?.GetComponent<Text>();
        levelText = GameObject.Find("Level")?.GetComponent<Text>();

        UpdateUI();
    }

    void ShowStartLevelCanvas()
    {
        if (startLevelCanvas != null)
        {
            startLevelCanvas.SetActive(true);
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

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
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
        UpdateUI();

        Debug.Log($"Balloon popped! {balloonsPopped}/{totalBalloons}");

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
        currentLevelIndex++;

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

        if (finalScoreText != null)
        {
            finalScoreText.text = "Here is your final score: " + score;
        }

        endMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SaveSessionData()
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

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levels[0]);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }

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

[Serializable]
public class SessionData
{
    public string PlayerID;
    public string StartTime;
    public string Duration;
    public int Score;
    public string Feedback;
}
