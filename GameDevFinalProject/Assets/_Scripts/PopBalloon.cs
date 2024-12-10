using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopBalloon : MonoBehaviour
{
    private string balloonTag = "Balloon";
    private GameManager gameManager;
    public GameObject balloonPopAnimation;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(balloonTag))
        {
            gameManager.BalloonPopped();

            if (balloonPopAnimation != null)
            {
                Instantiate(balloonPopAnimation, other.transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Pop animation prefab not assigned!");
            }

            Destroy(other.gameObject);
        }
    }
}
