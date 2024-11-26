using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    void Update()
    {
        // Get the mouse position in screen coordinates
        Vector3 mousePosition = Input.mousePosition;

        // Convert the mouse position to world coordinates
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Ensure the GameObject remains on the same z-plane
        mousePosition.z = transform.position.z;

        // Update the GameObject's position
        transform.position = mousePosition;
    }
}
