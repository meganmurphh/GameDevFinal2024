using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public Text livesText;
    public Slider timerSlider;
    public Text timerText;
    public Text levelText;
    public Text finalScoreText;
    public InputField feedbackInputField;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
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
        if (feedbackInputField == null)
            feedbackInputField = GameObject.Find("FeedbackInputField")?.GetComponent<InputField>();
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
    }

    public string GetFeedback()
    {
        if (feedbackInputField != null)
        {
            return feedbackInputField.text;
        }
        return "No feedback provided";
    }
}
