using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Text scoreText;
    public Text livesText;
    public Slider timerSlider;
    public Text timerText;
    public Text levelText;

    void Awake()
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
}
