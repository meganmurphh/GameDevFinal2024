using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For scene management

public class GameManager : MonoBehaviour
{
    public GameObject[] balloons; // Array to hold references to all the balloon objects
    //public string nextSceneName = "NextLevel"; // The scene to load after all balloons are popped

    private int balloonsPopped = 0; // Keeps track of how many balloons have been popped

    // Call this method from the PopBalloon script when a balloon is popped
    public void BalloonPopped()
    {
        // Increment the number of balloons popped
        balloonsPopped++;
        Debug.Log("balloons popped:" + balloonsPopped);

        // Check if all balloons have been popped
        if (balloonsPopped == balloons.Length)
        {
            // All balloons have been popped
            Debug.Log("All balloons popped! Level Complete!");

            // Optionally, load the next scene
          //  LoadNextLevel();
        }
    }

    // Method to load the next level
    //void LoadNextLevel()
  //  {
   //     SceneManager.LoadScene(nextSceneName);
   // }
}
