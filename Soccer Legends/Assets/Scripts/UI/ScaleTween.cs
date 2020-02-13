using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTween : MonoBehaviour
{
    public float time = 0;
    public Vector3 finalScale;

    Vector3 initialScale;

    private void Start()
    {
        initialScale = gameObject.transform.localScale;
    }
    public void Scale()
    {
        LeanTween.scale(gameObject, finalScale, time);
    }
    public void ResetScale()
    {
        LeanTween.scale(gameObject, initialScale, time);
    }
}
