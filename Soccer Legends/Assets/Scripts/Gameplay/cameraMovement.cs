using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    private Vector3 startTouch;
    private Vector3 startTouchWorld;
    private Vector3 lastCamPosition;
    private Vector3 localCamPos;
    Vector3 cameraBallOffset = Vector3.zero;
    Vector3 offset;
    PVE_Manager mg;
    public int fingerIdx;
    Vector3 aux;

    GameObject canvas;

    bool moved = false;

    // Start is called before the first frame update
    void Start()
    {
        localCamPos = transform.localPosition;
        transform.localPosition = new Vector3(0, 0, -0.5f);
        mg = GameObject.Find("Manager").GetComponent<PVE_Manager>();
        fingerIdx = -1;

        canvas = GameObject.Find("Canvas");
        canvas.GetComponent<Canvas>().worldCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mg.GameStarted) { lastCamPosition = transform.position; return; }
        if (mg.GameOn) ProcessInputs();
        else { 
        if(cameraBallOffset.y == -3) transform.position = Vector3.Lerp(lastCamPosition, transform.parent.position + new Vector3(0, 1.5f, transform.position.z), Time.deltaTime);
        else transform.position = Vector3.Lerp(lastCamPosition, transform.parent.position + new Vector3(0, 2.5f, transform.position.z), Time.deltaTime);
            lastCamPosition = transform.position;
        }
    }

    private void ProcessInputs()
    {
        //Movement
        if (!moved && Input.GetMouseButton(0))
        {
            aux = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                
                startTouchWorld = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)));
                offset = transform.position - startTouchWorld;
                startTouch = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0) && Vector2.Distance(Input.mousePosition, startTouch) > 30)
            {
                Vector3 camPos = offset + startTouchWorld;//transform.parent.position + lastCamPosition - startTouchWorld;
                float dist = Vector2.Distance(aux, startTouch) / Screen.width * 5;
                camPos += (startTouch - aux).normalized * dist;

                //ransform.position = startTouch + (startTouch - camPos);
                //if(putZAxis(Camera.main.ScreenToWorldPoint(startTouch)))
                transform.position = putZAxis(camPos);// + startTouchWorld);

                //ransform.position = Vector3.Lerp(lastCamPosition, camPos, 2 * Time.deltaTime);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            startTouchWorld = startTouch = Vector3.zero;
            lastCamPosition = transform.position;
            moved = false;
        }
        else if (!Input.GetMouseButton(0))
        {

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
