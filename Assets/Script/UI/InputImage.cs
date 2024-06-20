using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputImage : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image image;

    public UnityEvent<float> onPinchEvent;
    public UnityEvent<Vector2> onDragEvent;
    public UnityEvent<Vector2> onClickEvent;

    int touchCount;

    void Start()
    {
        image.sprite = null;
        image.color = new Color(0f, 0f, 0f, 0f);
        image.raycastTarget = true;

        touchCount = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (touchCount < 2 && onDragEvent != null)
        {
            onDragEvent.Invoke(eventData.delta / -1000f);

            Debug.Log("Drag " + (eventData.delta / -1000f).ToString());
        }
        else if (touchCount > 1 && onPinchEvent != null)
        {
            if (eventData.position.x < 0)
                onPinchEvent.Invoke(-eventData.delta.x);
            else
                onPinchEvent.Invoke(eventData.delta.x);

            Debug.Log("Pinch " + (eventData.delta.x/10f).ToString());
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touchCount -= 1;

        if (onClickEvent != null)
            onClickEvent.Invoke(eventData.delta);

        Debug.Log("Click " + eventData.delta.ToString());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchCount += 1;
    }
}
