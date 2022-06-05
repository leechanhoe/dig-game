using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyPad : MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    static public JoyPad instance;
    public RectTransform rectBackground;
    public RectTransform rectJoystick;
    Player player;

    float radius;
    public float angle;
    public bool isTouch;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        rectBackground = GetComponent<RectTransform>();
        radius = rectBackground.rect.width * 0.5f;
    }

    public void Transparency0()
    {
        Color color = rectBackground.GetComponent<Image>().color;
        color.a = 0;
        rectBackground.GetComponent<Image>().color = color;
        rectJoystick.GetComponent<Image>().color = color;
    }

    public void Transparency100()
    {
        Color color = rectBackground.GetComponent<Image>().color;
        color.a = 0.8f;
        rectBackground.GetComponent<Image>().color = color;
        rectJoystick.GetComponent<Image>().color = color;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 value = new Vector2(eventData.position.x - rectBackground.position.x, eventData.position.y - rectBackground.position.y);
        if (value.magnitude < radius / 2)
        {
            isTouch = false;
            player.DirectionInitialize();
        }
        else
            isTouch = true;
        value = Vector2.ClampMagnitude(value, radius);
        rectJoystick.localPosition = value;

        angle = Quaternion.FromToRotation(Vector3.up, value).eulerAngles.z; // 시계 반대방향으로 증가하는 360도 체계
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 value = new Vector2(eventData.position.x - rectBackground.position.x, eventData.position.y - rectBackground.position.y);

        if (value.magnitude < radius / 2)
        {
            isTouch = false;
            player.DirectionInitialize();
        }
        else
            isTouch = true;
        value = Vector2.ClampMagnitude(value, radius);
        rectJoystick.localPosition = value;

        angle = Quaternion.FromToRotation(Vector3.up, value).eulerAngles.z;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouch = false;
        player.DirectionInitialize();
        rectJoystick.localPosition = Vector3.zero;
    }
}
