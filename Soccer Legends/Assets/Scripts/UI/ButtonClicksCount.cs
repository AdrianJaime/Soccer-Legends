using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class ButtonClicksCount : MonoBehaviour, IPointerClickHandler
{
    public int tap;
    public UnityEvent myEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        tap = eventData.clickCount;

        if (tap == 2)
        {
            myEvent.Invoke();
        }

    }
}
