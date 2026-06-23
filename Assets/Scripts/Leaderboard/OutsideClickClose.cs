using UnityEngine;
using UnityEngine.EventSystems;

public class OutsideClickClose : MonoBehaviour, IPointerClickHandler
{
    public SlidePanel slidePanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        slidePanel.ClosePanel();
    }
}