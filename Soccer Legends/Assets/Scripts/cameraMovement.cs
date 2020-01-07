using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    private Vector3 startTouch;
    private Vector3 lastCamPosition;
    private Vector3 localCamPos;

    // Start is called before the first frame update
    void Start()
    {
        localCamPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
    }

    private void ProcessInputs()
    {
        //Movement
        if (Input.touchCount > 0)
        {
            Touch swipe = Input.GetTouch(0);
            Vector3 actualTouch = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
            if (swipe.phase == TouchPhase.Began)
            {
                startTouch = actualTouch;
            }

            else if (swipe.phase == TouchPhase.Moved)
            {
                Vector3 camPos;
                float dist = Vector2.Distance(actualTouch, startTouch);
                camPos = new Vector3(actualTouch.x, actualTouch.y, transform.position.z);
                if (dist > 1.0f)
                {
                    camPos += (startTouch - actualTouch).normalized * (dist - 1.0f);
                }
                //transform.position = startTouch + (startTouch - camPos);
                transform.position = camPos;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(lastCamPosition, transform.parent.position + localCamPos, 2 * Time.deltaTime);
            lastCamPosition = transform.position;
        }
    }

    private Vector3 putZAxis(Vector3 vec) { return new Vector3(vec.x, vec.y, 0); }
}
