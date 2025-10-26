using UnityEngine;
using UnityEngine.EventSystems;

public class HoverRotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float transitionSpeed = 10f;
    [SerializeField] private float hoverRotation = 10f; // Rotation in degrees
    
    private Quaternion originalRotation;
    private Quaternion targetRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalRotation = transform.localRotation;
        targetRotation = originalRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Smoothly interpolate between current rotation and target rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * transitionSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if ( GameManager.inst.cardsMoving )
            return;
        if ( GameManager.inst.currentTurn == GameManager.CurrentTurn.Player1 )
            targetRotation = originalRotation * Quaternion.Euler(0, 0, hoverRotation);
        else
            targetRotation = originalRotation * Quaternion.Euler(0, 0, -hoverRotation);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetRotation = originalRotation;
    }
}
