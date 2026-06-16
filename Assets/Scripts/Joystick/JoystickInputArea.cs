using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class JoystickInputArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] OnScreenStick joystick;
    [SerializeField] CanvasGroup joystickRoot;

    private bool active;

    void Start()
    {
        joystickRoot.alpha = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        active = true;

        joystickRoot.alpha = 1;
        joystickRoot.transform.position = eventData.position;
        joystick.OnPointerDown(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!active)
            return;

        joystick.OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!active)
            return;

        active = false;
        joystick.OnPointerUp(eventData);
        joystickRoot.alpha = 0;
    }
}