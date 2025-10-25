using UnityEngine;

public class CardDrag : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private float zDistanceToCamera;

    void OnMouseDown()
    {
        isDragging = true;

        // Find how far the card is from the camera
        zDistanceToCamera = Camera.main.WorldToScreenPoint(transform.position).z;

        // Get offset between mouse and card center
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = zDistanceToCamera;
        offset = transform.position - Camera.main.ScreenToWorldPoint(mousePosition);
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = zDistanceToCamera;
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
            transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}
