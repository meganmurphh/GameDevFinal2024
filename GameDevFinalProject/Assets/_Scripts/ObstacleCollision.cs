using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bomb"))
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
