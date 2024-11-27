using UnityEngine;

public class StartScreenManager : MonoBehaviour
{
    public GameObject startScreenCanvas;
   // public GameObject pauseMenuCanvas;
    public bool IsStartScreenActive => startScreenCanvas.activeSelf;

    private bool isPaused = false;

    void Start()
    {
        startScreenCanvas.SetActive(true);
       // pauseMenuCanvas.SetActive(false);
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        startScreenCanvas.SetActive(false);
       // pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    //public void TogglePauseGame()
    //{
        //isPaused = !isPaused;
        //Time.timeScale = isPaused ? 0f : 1f;

        //if (pauseMenuCanvas != null)
       // {
         //   pauseMenuCanvas.SetActive(isPaused);
       // }

  //  }

    public void ShowStartScreen()
    {
        startScreenCanvas.SetActive(true);
    }
}
