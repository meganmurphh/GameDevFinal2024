using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // UI Elements
    public Text scoreText;
    public Text livesText;
    public Text timerText;
    public Text levelText;
    public Text finalScoreText;

    public Slider timerSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {

        Debug.Log("UIManager Initialize called.");

        if (finalScoreText == null)
        {
            if (GameObject.Find("EndScreenCanvas") == null)
            {
                Debug.LogError("EndScreenCanvas is missing or not found in the scene.");
                return;
            }

            var endScreenCanvas = GameObject.Find("EndScreenCanvas");
            if (endScreenCanvas != null && !endScreenCanvas.activeSelf)
            {
                endScreenCanvas.SetActive(true);
            }

            finalScoreText = endScreenCanvas.transform.Find("Final Score Text")?.GetComponent<Text>();

            if (finalScoreText != null)
            {
                Debug.Log("FinalScoreText found and assigned.");
            }
            else
            {
                Debug.LogError("FinalScoreText is missing or not found in EndScreenCanvas.");
            }
        }
        else
        {
            Debug.Log("FinalScoreText already assigned.");
        }

        if (scoreText == null)
            scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();
        Debug.Log(scoreText == null ? "ScoreText not found." : "ScoreText found and assigned.");

        if (livesText == null)
            livesText = GameObject.Find("LivesText")?.GetComponent<Text>();
        Debug.Log(livesText == null ? "LivesText not found." : "LivesText found and assigned.");

        if (timerText == null)
            timerText = GameObject.Find("TimerText")?.GetComponent<Text>();
        Debug.Log(timerText == null ? "TimerText not found." : "TimerText found and assigned.");

        if (levelText == null)
            levelText = GameObject.Find("LevelText")?.GetComponent<Text>();
        Debug.Log(levelText == null ? "LevelText not found." : "LevelText found and assigned.");

        if (finalScoreText == null)
            finalScoreText = GameObject.Find("Final Score Text")?.GetComponent<Text>();
        Debug.Log(levelText == null ? "FinalScoreText not found." : "FinalScoreText found and assigned.");

    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        else
        {
            Debug.LogError("ScoreText is not assigned in the scene.");
        }
    }

    public void UpdateTimer(float timeRemaining, float maxTime)
    {
        if (timerSlider != null)
        {
            timerSlider.maxValue = maxTime;
            timerSlider.value = timeRemaining;
        }
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            Debug.LogError("TimerText is not assigned in the scene.");
        }
    }

    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
        else
        {
            Debug.LogError("LivesText is not assigned in the scene.");
        }
    }

    public void UpdateLevelNumber(int levelNumber)
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + levelNumber;
        }
        else
        {
            Debug.LogError("LevelText is not assigned in the scene.");
        }
    }

    public void DisplayFinalScore(int finalScore)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + finalScore;
        }
        else
        {
            Debug.LogError("FinalScoreText is not assigned in the scene.");
        }
    }
}