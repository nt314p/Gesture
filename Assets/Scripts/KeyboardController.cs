using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private Transform backgroundTransform;
    private bool dragging;
    private Vector2 offset;
    private Vector2 backgroundHalfScale;

    private Rect screenBounds;
    
    private void Start()
    {
        var localScale = backgroundTransform.localScale;
        boxCollider2D.size = localScale;
        backgroundHalfScale = 0.5f * ((Vector2)localScale);
        
        var halfScreenDimensions = mainCamera.ScreenToWorldPoint(new Vector3(0, 0));
        screenBounds.x = -halfScreenDimensions.x;
        screenBounds.y = -halfScreenDimensions.y;
        screenBounds.width = halfScreenDimensions.x * 2;
        screenBounds.height = halfScreenDimensions.y * 2;
    }

    private void Update()
    {
        if (dragging)
            transform.position = GetWorldSpaceMousePosition() + offset;
    }

    private void OnMouseDown()
    {
        dragging = true;
        offset = (Vector2)transform.position - GetWorldSpaceMousePosition();
    }

    private void OnMouseUp()
    {
        dragging = false;
        if (transform.position.x - backgroundHalfScale.x < screenBounds.xMax)
        {
            transform.position = new Vector3(screenBounds.xMax + backgroundHalfScale.x, transform.position.y);
        }
        if (transform.position.y - backgroundHalfScale.y < screenBounds.yMax)
        {
            transform.position = new Vector3(transform.position.x, screenBounds.yMax + backgroundHalfScale.y);
        }
        
        if (transform.position.x + backgroundHalfScale.x > screenBounds.xMin)
        {
            transform.position = new Vector3(screenBounds.xMin - backgroundHalfScale.x, transform.position.y);
        }
        if (transform.position.y + backgroundHalfScale.y > screenBounds.yMin)
        {
            transform.position = new Vector3(transform.position.x, screenBounds.yMin - backgroundHalfScale.y);
        }
    }

    private Vector2 GetWorldSpaceMousePosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
}
