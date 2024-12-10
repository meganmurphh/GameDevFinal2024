using UnityEngine;

public class ObstacleCollision : MonoBehaviour
{
    public GameObject explosionAnimation;
    public GameObject elimZoneAnimation;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bomb"))
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.PlayerHitObstacle();

                if (explosionAnimation != null)
                {
                    Instantiate(explosionAnimation, other.transform.position, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("bomb animation prefab not assigned");
                }

                gameObject.SetActive(false);
                Destroy(other.gameObject);

                Invoke("ShowBirdAfterDelay", 1.5f);
            }
            else
            {
                Debug.LogError("GameManager not found!");
            }
        }

        if (other.CompareTag("ElimZone"))
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.PlayerHitObstacle();

                if (elimZoneAnimation != null)
                {
                    Instantiate(elimZoneAnimation, other.transform.position, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("elim zone animation prefab not assigned");
                }
            }
            else
            {
                Debug.LogError("GameManager not found!");
            }
        }
    }

    void ShowBirdAfterDelay()
    {
        gameObject.SetActive(true);
    }
}
