using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public float followSpeed = 20f;
    public float checkRadius = 0.3f;
    public LayerMask mazeLayer;

    private Transform spriteTransform;

    void Start()
    {
        spriteTransform = transform.GetChild(0);
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = spriteTransform.position.z;

        Vector3 directionToMouse = mousePosition - spriteTransform.position;

        Vector3 targetPosition = spriteTransform.position + directionToMouse.normalized * followSpeed * Time.deltaTime;

        if (!IsCollidingWithMaze(targetPosition))
        {
            spriteTransform.position = Vector3.MoveTowards(spriteTransform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            Debug.Log("Collision detected at: " + targetPosition);
        }
    }

    bool IsCollidingWithMaze(Vector3 targetPosition)
    {
        Collider2D collider = Physics2D.OverlapCircle(targetPosition, checkRadius, mazeLayer);

        return collider != null;
    }
}
