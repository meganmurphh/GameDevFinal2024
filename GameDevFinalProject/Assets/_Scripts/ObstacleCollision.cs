using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bird")) // Ensure player tag matches
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.PlayerHitObstacle(); // Notify GameManager
            }
            else
            {
                Debug.LogError("GameManager not found!");
            }
        }
    }
}
