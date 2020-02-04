using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonDuration : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{

    public float timeToHold=1f;
    public float spearationRatio = 3f;
    bool isDown;
    float timeDown;
    Vector2 positionDown;
    public UnityEvent onButtonHold;

    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
        timeDown = Time.time;

        positionDown = Input.GetTouch(0).position;
        Debug.Log(positionDown);
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
            if (Vector2.Distance(positionDown, Input.GetTouch(0).position) > spearationRatio)
            {
                Debug.Log(Vector2.Distance(positionDown, Input.GetTouch(0).position));
                ResetParameters();
            }
        }
    }

    void ResetParameters()
    {
        isDown = false;
        timeDown = 0;
    }


}
