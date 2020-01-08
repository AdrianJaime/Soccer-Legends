using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    private Vector3 startTouch;
    private Vector3 lastCamPosition;
    private Vector3 localCamPos;
    PVE_Manager mg;
    public int fingerIdx;

    // Start is called before the first frame update
    void Start()
    {
        localCamPos = transform.localPosition;
        mg = GameObject.Find("Manager").GetComponent<PVE_Manager>();
        fingerIdx = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (mg.GameOn) ProcessInputs();
        else if (fingerIdx != -1 && Input.GetTouch(fingerIdx).phase == TouchPhase.Ended)
        {
            mg.releaseTouchIdx(fingerIdx);
            fingerIdx = -1;
        }
        }

    private void ProcessInputs()
    {
        //Movement
        if ((Input.touchCount > mg.getTotalTouches() || fingerIdx != -1))
        {
            if (fingerIdx == -1) fingerIdx = mg.getTouchIdx();
            Touch swipe = Input.GetTouch(fingerIdx);
            Vector3 actualTouch = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
            if (swipe.phase == TouchPhase.Began)
            {
                startTouch = actualTouch;
            }

            else if (swipe.phase == TouchPhase.Moved)
            {
                Vector3 camPos = actualTouch;
                float dist = Vector3.Distance(actualTouch, startTouch) / 2;
                camPos = lastCamPosition + (actualTouch - startTouch).normalized * dist;
                if (dist > 5.0f)
                {
                    camPos += (startTouch - camPos).normalized * (dist - 5.0f);
                }

                //ransform.position = startTouch + (startTouch - camPos);
                transform.position = camPos;
                //ransform.position = Vector3.Lerp(lastCamPosition, camPos, 2 * Time.deltaTime);
                //lastCamPosition = transform.position;
            }
            else if(swipe.phase == TouchPhase.Ended)
            {
                mg.releaseTouchIdx(fingerIdx);
                fingerIdx = -1;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(lastCamPosition, transform.parent.position + localCamPos, Time.deltaTime);
            lastCamPosition = transform.position;
        }
    }

    private Vector3 putZAxis(Vector3 vec) { return new Vector3(vec.x, vec.y, transform.position.z); }
}
