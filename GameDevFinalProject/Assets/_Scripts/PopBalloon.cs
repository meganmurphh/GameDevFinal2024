using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopBalloon : MonoBehaviour
{
    public string balloonTag = "Balloon";
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(balloonTag))
        {
            gameManager.BalloonPopped();
            Destroy(other.gameObject);
        }
    }
}
