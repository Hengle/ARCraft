using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public UnityEvent onPointerDown, onPointerUp;

    public void OnPointerDown(PointerEventData eventData) {
        onPointerDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData) {
        onPointerUp.Invoke();
    }
}
