using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private UIManager uiManager;

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
    private bool sessionEnded = false;

    private DateTime sessionStartTime;
    private string playerID = "Player_" + Guid.NewGuid();

    public string[] levels;
    private int currentLevelIndex = 0;

    public GameObject levelCompleteCanvas;
    public GameObject startLevelCanvas;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
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

        uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found! Ensure the UIManager is in the scene.");
            return;
        }

        if (balloonsParent != null)
        {
            totalBalloons = balloonsParent.transform.childCount;
        }

        ResetPlayerPosition();
        UpdateUI();

        if (currentLevelIndex > 0)
        {
            ShowStartLevelCanvas();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            uiManager.UpdateTimer(remainingTime, sessionDuration);
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
        ResetPlayerPosition();
        uiManager.UpdateLevelNumber(currentLevelIndex + 1);
        UpdateUI();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void ShowStartLevelCanvas()
    {
        if (startLevelCanvas != null)
        {
            startLevelCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void StartLevel()
    {
        startLevelCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void PlayerHitObstacle()
    {
        lives--;

        if (lives > 0)
        {
            ResetPlayerPosition();
        }
        else
        {
            EndSession();
        }

        uiManager.UpdateLives(lives);
    }

    public void BalloonPopped()
    {
        balloonsPopped++;
        score++;

        uiManager.UpdateScore(score);

        if (balloonsPopped == totalBalloons)
        {
            ShowLevelCompleteCanvas();
        }
    }

    void ShowLevelCompleteCanvas()
    {
        levelCompleteCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    void ResetPlayerPosition()
    {
        if (player != null && playerStartPosition != null)
        {
            player.transform.position = playerStartPosition.position;
        }
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levels.Length)
        {
            uiManager.UpdateLevelNumber(currentLevelIndex + 1);
            string nextSceneName = levels[currentLevelIndex];
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            EndSession();
        }
    }

    void EndSession()
    {
        if (!sessionEnded)
        {
            sessionEnded = true;

            uiManager.DisplayFinalScore(score);
            SaveSessionData();

            Time.timeScale = 0f;
        }
    }

    public void SaveSessionData()
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, "SessionData.json");
            File.WriteAllText(filePath, JsonUtility.ToJson(new SessionData
            {
                PlayerID = playerID,
                StartTime = sessionStartTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Duration = (sessionDuration - remainingTime).ToString("F2") + " seconds",
                Score = score,
                Feedback = uiManager.GetFeedback()
            }, true));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save session data: {ex.Message}");
        }
    }

    void UpdateUI()
    {
        uiManager.UpdateScore(score);
        uiManager.UpdateLives(lives);
        uiManager.UpdateTimer(remainingTime, sessionDuration);
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
