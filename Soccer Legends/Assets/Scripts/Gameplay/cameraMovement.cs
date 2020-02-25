﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    private Vector3 startTouch;
    private Vector3 startTouchWorld;
    private Vector3 lastCamPosition;
    private Vector3 localCamPos;
    Vector3 offset;
    PVE_Manager mg;
    public int fingerIdx;

    GameObject canvas;
    strategyUI guiInteractions;

    // Start is called before the first frame update
    void Start()
    {
        localCamPos = transform.localPosition;
        mg = GameObject.Find("Manager").GetComponent<PVE_Manager>();
        fingerIdx = -1;

        canvas = GameObject.Find("Canvas");
        guiInteractions = canvas.transform.GetChild(5).GetComponent <strategyUI>();
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
        bool found = false;
        if (fingerIdx == -1)
        {
            foreach (Touch t in Input.touches)
            {
                if (t.phase == TouchPhase.Moved)
                {
                    foreach (GameObject p in mg.myPlayers)
                    {
                        if (p.GetComponent<MyPlayer_PVE>().fingerIdx != t.fingerId) found = true;
                        else
                        {
                            found = false;
                            break;
                        }
                    }
                }
            }
        }
        //Movement
        if ((Input.touchCount > mg.getTotalTouches() && found || fingerIdx != -1))
        {
            if (fingerIdx == -1) fingerIdx = mg.getTouchIdx();
            Touch swipe = Input.GetTouch(fingerIdx);
            Vector3 actualTouch = new Vector3(swipe.position.x, swipe.position.y, 0);
            if (startTouch == Vector3.zero)
            {
                startTouch = actualTouch;
                startTouchWorld = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
                offset = transform.position - startTouchWorld;
                //if(startTouchWorld.x < transform.position.x) startTouchWorld = new Vector3 ()
            }

            else if (swipe.phase == TouchPhase.Moved && !guiInteractions.isInteracting())
            {
                Vector3 camPos = offset + startTouchWorld;//transform.parent.position + lastCamPosition - startTouchWorld;
                float dist = Vector3.Distance(actualTouch, startTouch) / Screen.width*5;
                camPos += (startTouch - actualTouch).normalized * dist;

                //ransform.position = startTouch + (startTouch - camPos);
                //if(putZAxis(Camera.main.ScreenToWorldPoint(startTouch)))
                transform.position = putZAxis(camPos);// + startTouchWorld);
                
                //ransform.position = Vector3.Lerp(lastCamPosition, camPos, 2 * Time.deltaTime);
            }
            else if(swipe.phase == TouchPhase.Ended)
            {
                mg.releaseTouchIdx(fingerIdx);
                fingerIdx = -1;
                startTouchWorld = startTouch = Vector3.zero;
                lastCamPosition = transform.position;
            }
        }
        else
        {
            Vector3 cameraBallOffset = Vector3.zero;
            switch (mg.HasTheBall())
            {
                case 0:
                    cameraBallOffset = Vector3.zero;
                    break;
                case 1:
                    cameraBallOffset = new Vector3(0, 3, 0);
                    break;
                case 2:
                    cameraBallOffset = new Vector3(0, -3, 0);
                    break;
            }
            transform.position = Vector3.Lerp(lastCamPosition, transform.parent.position + localCamPos + cameraBallOffset, Time.deltaTime);
            lastCamPosition = transform.position;
        }
    }

    private Vector3 putZAxis(Vector3 vec) { return new Vector3(vec.x, vec.y, transform.position.z); }
}