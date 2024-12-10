using System;
using System.IO;
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

        Debug.Log($"Balloon popped! {balloonsPopped}/{totalBalloons} balloons collected.");

        UpdateUI();

        if (balloonsPopped == totalBalloons)
        {
            Debug.Log("All balloons collected! Advancing to the next level.");
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

        UpdateUI();
    }

    public void ResetPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Bird");

        if (player != null && playerStartPosition != null)
        {
            var followMouse = player.GetComponent<FollowMouse>();
            if (followMouse != null)
            {
                followMouse.ResetPosition(playerStartPosition.position);
            }
            else
            {
                player.transform.position = playerStartPosition.position;
            }
        }
        else if (player == null)
        {
            Debug.LogError("Player GameObject not found in the scene!");
        }
        else if (playerStartPosition == null)
        {
            Debug.LogError("PlayerStartPosition not assigned in GameManager!");
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
