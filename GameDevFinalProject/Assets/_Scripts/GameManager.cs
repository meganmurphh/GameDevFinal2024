using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject startScreenUI;
    public void StartGame()
    {

        startScreenUI.SetActive(false);

        // Load the first scene (replace with the name of your first scene)
        SceneManager.LoadScene("StartingScene");
    }
}
