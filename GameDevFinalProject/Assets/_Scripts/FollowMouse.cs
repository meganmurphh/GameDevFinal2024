using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public float followSpeed = 20f;
    public float stoppingDistance = 0.05f; // Threshold to stop moving toward the cursor
    public LayerMask mazeLayer;

    private Rigidbody2D rb;
    private Transform spriteTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        spriteTransform = transform.GetChild(0);
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = spriteTransform.position.z;

        Vector3 directionToMouse = mousePosition - spriteTransform.position;
        float distanceToMouse = directionToMouse.magnitude;

        if (distanceToMouse > stoppingDistance)
        {
            // Move toward the cursor if outside the stopping distance
            Vector3 targetPosition = spriteTransform.position + directionToMouse.normalized * followSpeed * Time.deltaTime;

            if (!IsCollidingWithMaze(targetPosition))
            {
                rb.MovePosition(Vector3.MoveTowards(rb.position, targetPosition, followSpeed * Time.deltaTime));
            }
        }
        else
        {
            // Snap to cursor position if within the stopping distance
            rb.velocity = Vector2.zero;
            rb.position = mousePosition;
        }
    }

    bool IsCollidingWithMaze(Vector3 targetPosition)
    {
        Collider2D collider = Physics2D.OverlapCircle(targetPosition, stoppingDistance, mazeLayer);
        return collider != null;
    }
}
