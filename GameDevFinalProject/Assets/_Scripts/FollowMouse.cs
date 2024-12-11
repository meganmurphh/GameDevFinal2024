using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public float followSpeed = 20f;
    public float checkRadius = 0.3f;
    public float deadZoneRadius = 0.1f;
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

        if (directionToMouse.magnitude > deadZoneRadius)
        {
            Vector3 targetPosition = spriteTransform.position + directionToMouse.normalized * followSpeed * Time.deltaTime;

            if (!IsCollidingWithMaze(targetPosition))
            {
                rb.MovePosition(Vector3.MoveTowards(rb.position, targetPosition, followSpeed * Time.deltaTime));
            }

        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void ResetPosition(Vector3 newPosition)
    {
        rb.position = newPosition;
        rb.velocity = Vector2.zero;
    }

    bool IsCollidingWithMaze(Vector3 targetPosition)
    {
        Collider2D collider = Physics2D.OverlapCircle(targetPosition, checkRadius, mazeLayer);
        return collider != null;
    }
}
