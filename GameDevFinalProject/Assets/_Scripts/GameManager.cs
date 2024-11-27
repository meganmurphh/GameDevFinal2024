using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject balloonsParent;
    public string nextSceneName;

    private int totalBalloons = 0; // Total number of balloons
    private int balloonsPopped = 0; // Number of balloons popped

    void Start()
    {
        // Get the total number of balloons (children of the Balloons parent)
        totalBalloons = balloonsParent.transform.childCount;
    }

    // Call this method from the PopBalloon script when a balloon is popped
    public void BalloonPopped()
    {
        // Increment the number of balloons popped
        balloonsPopped++;

        // Check if all balloons have been popped
        if (balloonsPopped == totalBalloons)
        {
            // All balloons have been popped
            Debug.Log("All balloons popped! Level Complete!");

            // Optionally, load the next scene
            LoadNextLevel();
        }
    }

    // Method to load the next level
    void LoadNextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
