using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    Vector2[] players;
    Vector2 Ball;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.parent.transform.childCount - 1; i++) players[i] = transform.parent.transform.GetChild(i).transform.position;
        if (GameObject.FindGameObjectWithTag("Ball") != null)
        {
            Ball = GameObject.FindGameObjectWithTag("Ball").transform.position;
            transform.position = MidPoint();
        }
    }

    private Vector2 MidPoint()
    {
        return new Vector2((players[0].x + Ball.x) / 2, (players[0].y + Ball.y) / 2);
    }
}
