using UnityEngine;
using System.Collections;

public class StartScreenManager : MonoBehaviour
{
    public GameObject startScreenCanvas;
    public GameObject player;

    private FollowMouse followMouseScript;

    void Start()
    {
        if (player != null)
        {
            followMouseScript = player.GetComponent<FollowMouse>();
            if (followMouseScript != null)
            {
                followMouseScript.enabled = false;
            }
        }

        startScreenCanvas.SetActive(true);
        Time.timeScale = 0f;
    }
    public void StartGame()
    {
        startScreenCanvas.SetActive(false);
        Time.timeScale = 1f;

        if (followMouseScript != null)
        {
            StartCoroutine(EnablePlayerMovementAfterDelay(0.5f));
        }
    }

    public void ShowStartScreen()
    {
        startScreenCanvas.SetActive(true);

        if (followMouseScript != null)
        {
            followMouseScript.enabled = false; 
        }

        Time.timeScale = 0f;
    }

    private IEnumerator EnablePlayerMovementAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (followMouseScript != null)
        {
            followMouseScript.enabled = true; 
        }
    }
}
