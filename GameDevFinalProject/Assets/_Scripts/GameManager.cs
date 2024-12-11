using System;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private UIManager uiManager;

    public GameObject balloonsParent;
    public Transform playerStartPosition;
    public int totalLives = 3;
    public float sessionDuration = 240f;

    private int score = 0;
    private int lives;
    private float remainingTime;
    private bool sessionEnded = false;

    private int totalBalloons = 0;
    private int balloonsPopped = 0;

    public string[] levels = { "Level1", "Level2", "Level3", "Level4" };
    private int currentLevelIndex = 0;

    private bool isPaused = false;

    public GameObject player;
    private FollowMouse followMouseScript;

    public GameObject endMenuCanvas;
    public GameObject pauseScreenCanvas;
    public GameObject levelCompleteCanvas;
    public GameObject spawnBirdCanvas;

    public Text finalScoreText;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (GameData.CurrentLevel == 0)
        {
            GameData.Reset();
        }

        currentLevelIndex = GameData.CurrentLevel;
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

        UpdateUI();
    }

    void Update()
    {
        if (remainingTime > 0 && !sessionEnded)
        {
            remainingTime -= Time.deltaTime;

            if (uiManager != null)
            {
                uiManager.UpdateTimer(remainingTime, sessionDuration);
            }
        }
        else if (!sessionEnded)
        {
            EndSession();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        // Pause the game when the scene is loaded
        Time.timeScale = 0f;

        // Hide spawnBirdCanvas unless on the first level (or whichever level needs it)
        if (currentLevelIndex > 0) // This ensures that it's hidden on levels after the first one
        {
            if (spawnBirdCanvas != null)
            {
                spawnBirdCanvas.SetActive(true); // Show the canvas if necessary
            }
        }

        // Initialize balloons and other UI elements
        balloonsParent = GameObject.Find("BalloonsParent");
        if (balloonsParent == null)
        {
            Debug.LogError("BalloonsParent not found in the scene!");
        }
        else
        {
            totalBalloons = balloonsParent.transform.childCount;
            balloonsPopped = 0;
        }

        uiManager = UIManager.Instance;
        if (uiManager != null)
        {
            uiManager.Initialize();
            UpdateUI();
        }
    }


    public void BalloonPopped()
    {
        balloonsPopped++;
        score++;

        GameData.Score = score;
        UpdateUI();

        if (balloonsPopped == totalBalloons)
        {
            if (currentLevelIndex < levels.Length - 1)
            {
                ShowLevelCompleteCanvas();
            }
            else
            {
                EndSession();
            }
        }
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;

        GameData.Score = score;
        GameData.Lives = lives;
        GameData.RemainingTime = remainingTime;
        GameData.CurrentLevel = currentLevelIndex;

        if (currentLevelIndex < levels.Length)
        {
            string nextSceneName = levels[currentLevelIndex];
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("No more levels to load. Ending session.");
            EndSession();
        }
    }


    void EndSession()
    {
        sessionEnded = true;
        GameData.FinalScore = GameData.Score;
        GameData.Lives = 0;
        GameData.RemainingTime = 0;

        if (uiManager != null)
        {
            Debug.Log("Displaying final score...");
            finalScoreText.text = "Final Score: " + GameData.FinalScore;

        }
        else
        {
            Debug.LogError("UIManager is not initialized!");
        }

        SaveSessionData();
        if (endMenuCanvas != null)
        {
            endMenuCanvas.SetActive(true);
        }

        Time.timeScale = 0f;
    }


    public void SaveSessionData()
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, "SessionData.json");
            File.WriteAllText(filePath, JsonUtility.ToJson(new SessionData
            {
                PlayerID = Guid.NewGuid().ToString(),
                StartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Duration = (sessionDuration - remainingTime).ToString("F2") + " seconds",
                Score = score,
                FinalScore = GameData.FinalScore
            }, true));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save session data: {ex.Message}");
        }
    }

    public void UpdateUI()
    {
        if (uiManager == null)
        {
            Debug.LogError("UIManager is not available!");
            return;
        }

        uiManager.UpdateScore(GameData.Score);
        uiManager.UpdateLives(GameData.Lives);
        uiManager.UpdateTimer(GameData.RemainingTime, sessionDuration);
        uiManager.UpdateLevelNumber(GameData.CurrentLevel + 1);
    }

    void ShowLevelCompleteCanvas()
    {
        if (levelCompleteCanvas != null)
        {
            Debug.Log("Showing level complete canvas.");
            levelCompleteCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("Level complete canvas is not assigned!");
        }
    }


    public void PlayerHitObstacle()
    {
        lives--;
        GameData.Lives = lives;

        if (lives > 0)
        {
            ResetPlayerPosition();
            StartCoroutine(ShowBirdSpawnCanvasWithDelay());
        }
        else
        {
            EndSession();
        }

        UpdateUI();
    }

    private IEnumerator ShowBirdSpawnCanvasWithDelay()
    {
        yield return new WaitForSeconds(1.5f);
        ShowBirdSpawnCanvas();
    }

    void ShowBirdSpawnCanvas()
    {
        if (spawnBirdCanvas != null)
        {
            spawnBirdCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void StartLevel()
    {
        if (spawnBirdCanvas != null)
        {
            spawnBirdCanvas.SetActive(false); // Hide the spawn bird canvas
        }

        Time.timeScale = 1f; // Resume the game (start the timer)

        if (followMouseScript != null)
        {
            followMouseScript.enabled = true; // Enable bird movement
        }
    }


    void ResetPlayerPosition()
    {
        if (player != null && playerStartPosition != null)
        {
            player.transform.position = playerStartPosition.position;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pauseScreenCanvas != null)
            {
                pauseScreenCanvas.SetActive(true);
            }
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseScreenCanvas != null)
            {
                pauseScreenCanvas.SetActive(false);
            }
        }
    }

    public void ResumeGame()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            if (pauseScreenCanvas != null)
            {
                pauseScreenCanvas.SetActive(false);
            }
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }

    public void PlayAgain()
    {
        score = 0;
        lives = totalLives;
        remainingTime = sessionDuration;

        totalBalloons = balloonsParent.transform.childCount;
        balloonsPopped = 0;

        if (player != null && playerStartPosition != null)
        {
            player.transform.position = playerStartPosition.position;
        }

        if (endMenuCanvas != null)
        {
            endMenuCanvas.SetActive(false);
        }

        currentLevelIndex = 0;
        string levelToLoad = levels[currentLevelIndex];
        SceneManager.LoadScene(levelToLoad);

        Time.timeScale = 1f;

        InitializeGame();
    }

    [Serializable]
    public class SessionData
    {
        public string PlayerID;
        public string StartTime;
        public string Duration;
        public int Score;
        public int FinalScore;
    }
}