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
                    Debug.LogWarning("Explosion animation prefab not assigned");
                }

                // Deactivate the bird instead of destroying it
                other.gameObject.SetActive(false);

                // Reactivate bird after reset
                StartCoroutine(ReactivateBirdAfterDelay(other.gameObject, 1.5f));
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
                    Debug.LogWarning("Elim zone animation prefab not assigned");
                }
            }
            else
            {
                Debug.LogError("GameManager not found!");
            }
        }
    }

    private System.Collections.IEnumerator ReactivateBirdAfterDelay(GameObject bird, float delay)
    {
        yield return new WaitForSeconds(delay);
        bird.SetActive(true);
    }
}
