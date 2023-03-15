using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 
/// Atasat de orice obiect UI care dorim sa realizeze o actiune la Hover
/// 
/// </summary>

public class HoverAction : MonoBehaviour, ISelectHandler, IPointerEnterHandler {
    public UnityEvent onSelect;

    public void OnPointerEnter(PointerEventData eventData) {
        onSelect.Invoke();
    }

    public void OnSelect(BaseEventData eventData) {
        onSelect.Invoke();
    }
}
