
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonFlipFlop : MonoBehaviour
{
    public UnityEvent A;
    public UnityEvent B;
    private bool first = true;

    public void OnButtonClick()
    {
        if (first)
        {
            A.Invoke();
            first = false;
        }
        else
        {
            B.Invoke();
            first = true;
        }
    }
}
