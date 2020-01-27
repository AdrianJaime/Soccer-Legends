using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTween : MonoBehaviour
{
    public float time = 0;
    public Vector3 movement;

    Transform initialPosition;

    private void Start()
    {
        initialPosition = gameObject.transform;
    }
    public void Move()
    {
        LeanTween.move(gameObject, movement, time);
    }
    public void ResetMove()
    {
        LeanTween.moveY(gameObject,initialPosition.position.y, time);
    }
}
