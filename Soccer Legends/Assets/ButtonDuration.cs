using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonDuration : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{

    public float timeToHold=1f;

    bool isDown;
    float timeDown;

    public UnityEvent onButtonHold;

    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
        timeDown = Time.time;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDown)
        {
            isDown = false;
        }

    }
    private void Update()
    {
        if (isDown)
        {
            if (Time.time - timeDown > timeToHold)
            {
                if (onButtonHold != null)
                {
                    ResetParameters();
                    onButtonHold.Invoke();
                }
            }
        }
    }

    void ResetParameters()
    {
        isDown = false;
        timeDown = 0;
    }


}
