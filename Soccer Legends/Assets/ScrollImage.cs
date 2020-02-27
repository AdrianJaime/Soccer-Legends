using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollImage : MonoBehaviour
{

    public float scrollVel = 5;
    public float offset = 100f;
    Vector2 startPos;

    private void Start()
    {
        startPos = transform.position;
    }
    private void Update()
    {
        float newPos = Mathf.Repeat(Time.time * scrollVel, offset);
        transform.position = startPos + Vector2.right * newPos;
    }
}
