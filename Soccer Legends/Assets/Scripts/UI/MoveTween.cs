using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTween : MonoBehaviour
{
    public float time = 0;
    public Vector3 movement;

    Vector3 initialPosition;

    private void Start()
    {
        initialPosition = gameObject.transform.position;
    }
    public void Move()
    {
        LeanTween.move(gameObject, initialPosition+movement, time);
    }
    public void ResetMove()
    {
        LeanTween.moveY(gameObject,initialPosition.y, time);
    }
}
