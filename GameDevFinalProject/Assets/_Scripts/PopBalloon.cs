using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopBalloon : MonoBehaviour
{
    public string birdTag = "Bird";

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering the trigger has the "Bird" tag
        if (other.CompareTag(birdTag))
        {
            Debug.Log("Bird popped the balloon!"); // Make sure this log appears in the console

            // Destroy the balloon when the bird touches it
            Destroy(gameObject);
        }
    }
}
