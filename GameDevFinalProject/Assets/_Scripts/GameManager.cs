using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private UIManager uiManager;

    public GameObject balloonsParent;
    public Transform playerStartPosition; // Player start position reference
    public int totalLives = 3;
    public float sessionDuration = 240f;

    private int score = 0;
    private int lives;
    private int totalBalloons = 0;
    private int balloonsPopped = 0;
    private float remainingTime;
    private bool sessionEnded = false;

    private DateTime sessionStartTime;
    private string playerID = "Player_" + Guid.NewGuid();

    public string[] levels;
    private int currentLevelIndex = 0;

    private bool isPaused = false;

    public GameObject player;
    private FollowMouse followMouseScript;

    //Canvases
    public GameObject endMenuCanvas;
    public GameObject pauseMenuCanvas;
    public GameObject levelCompleteCanvas;
    public GameObject startLevelCanvas;

    void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        lives = totalLives;
        remainingTime = sessionDuration;
        sessionStartTime = DateTime.Now;

        if (playerStartPosition == null)
        {
            Debug.LogError("PlayerStartPosition is not assigned in GameManager!");
            return;
        }

        if (balloonsParent == null)
        {
            balloonsParent = GameObject.Find("BalloonsParent");
            if (balloonsParent == null)
            {
                Debug.LogError("BalloonsParent not found in the scene!");
                return;
            }
        }

        followMouseScript = player.GetComponent<FollowMouse>();
        if (followMouseScript != null && currentLevelIndex != 0)
        {
            followMouseScript.enabled = false; // Disable player movement initially
        }

        totalBalloons = balloonsParent.transform.childCount;
        Debug.Log($"Total balloons in this level: {totalBalloons}");

        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found! Ensure the UIManager is in the scene.");
            return;
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

        balloonsParent = GameObject.Find("BalloonsParent");
        if (balloonsParent != null)
        {
            totalBalloons = balloonsParent.transform.childCount;
            balloonsPopped = 0;
            Debug.Log($"Total balloons in this level: {totalBalloons}");
        }
        else
        {
            Debug.LogError("BalloonsParent not found in the new scene!");
        }

        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            UpdateUI();
        }
    }

    public void BalloonPopped()
    {
        balloonsPopped++;
        score++;

        UpdateUI();

        if (balloonsPopped == totalBalloons)
        {
            ShowLevelCompleteCanvas();
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levels.Length)
        {
            string nextSceneName = levels[currentLevelIndex];
            Debug.Log($"Loading next level: {nextSceneName}");
            SceneManager.sceneLoaded += OnSceneLoaded;
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

        if (uiManager != null)
        {
            uiManager.DisplayFinalScore(score);
        }

        SaveSessionData();
        endMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
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
                Feedback = uiManager?.GetFeedback() ?? "No feedback"
            }, true));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save session data: {ex.Message}");
        }
    }

    public void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(score);
            uiManager.UpdateLives(lives);
            uiManager.UpdateTimer(remainingTime, sessionDuration);
            uiManager.UpdateLevelNumber(currentLevelIndex + 1);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuCanvas.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }
    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PlayerHitObstacle()
    {
        lives--;

        if (lives > 0)
        {
            ResetPlayerPosition();
            StartCoroutine(ShowStartLevelCanvasWithDelay());
        }
        else
        {
            EndSession();
        }

        uiManager.UpdateLives(lives);
    }

    private IEnumerator ShowStartLevelCanvasWithDelay()
    {
        yield return new WaitForSeconds(1.5f);

        ShowStartLevelCanvas();
    }

    public void StartLevel()
    {
        startLevelCanvas.SetActive(false);
        Time.timeScale = 1f;

        if (followMouseScript != null)
        {
            followMouseScript.enabled = true;
        }
    }

    void ShowLevelCompleteCanvas()
    {
        levelCompleteCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    void ShowStartLevelCanvas()
    {
        if (startLevelCanvas != null)
        {
            startLevelCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void ResetPlayerPosition()
    {
        if (player != null && playerStartPosition != null)
        {
            player.transform.position = playerStartPosition.position;
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
