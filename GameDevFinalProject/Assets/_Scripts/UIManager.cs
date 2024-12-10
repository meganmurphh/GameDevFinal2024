using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Text scoreText;
    public Text livesText;
    public Slider timerSlider;
    public Text timerText;
    public Text levelText;
    public Text finalScoreText;

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
        if (scoreText == null)
            scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();
        if (livesText == null)
            livesText = GameObject.Find("LivesText")?.GetComponent<Text>();
        if (timerSlider == null)
            timerSlider = GameObject.Find("TimerSlider")?.GetComponent<Slider>();
        if (timerText == null)
            timerText = GameObject.Find("TimerText")?.GetComponent<Text>();
        if (levelText == null)
            levelText = GameObject.Find("LevelText")?.GetComponent<Text>();
        if (finalScoreText == null)
            finalScoreText = GameObject.Find("FinalScoreText")?.GetComponent<Text>();

        UpdateScore(GameData.Score);
        UpdateLives(GameData.Lives);
        UpdateTimer(GameData.RemainingTime, 240f);
        UpdateLevelNumber(GameData.CurrentLevel + 1);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
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
    }

    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
    }

    public void UpdateLevelNumber(int levelNumber)
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + levelNumber;
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
