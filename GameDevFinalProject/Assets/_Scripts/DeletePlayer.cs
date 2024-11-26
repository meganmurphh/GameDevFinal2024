using UnityEngine;

public class DeletePlayer: MonoBehaviour
{
    public string birdTag = "Bird";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(birdTag))
        {
            Debug.Log("Player entered the elimination zone!");

            Destroy(other.gameObject);
        }
    }
}
