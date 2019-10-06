using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;


public class MyPlayer : MonoBehaviourPun, IPunObservable
{
    public PhotonView pv;
    public float speed, dist, maxPointDist, minPointDist, characterRad;
    public int maxPoints;
    public GameObject playerCamera;
    public GameObject line;

    public bool onMove = false;
    private Vector3 smoothMove;
    private GameObject actualLine;
    private List<Vector3> points;

    private void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 15;
        if (photonView.IsMine)
        {
            GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
            playerCamera.SetActive(true);
        }
        points = new List<Vector3>();
    }
    private void Update()
    {
        if (photonView.IsMine) //Check if we're the local player
        {
            ProcessInputs();
            if (actualLine && points.Count > 1) FollowLine();
        }
        else if(!photonView.IsMine)
        {
            smoothMovement();
        }
    }

    private void smoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
    }

    private void ProcessInputs()
    {
        //Movement
        if (Input.touchCount > 0)
        {
            Touch swipe = Input.GetTouch(0);
            if (swipe.phase == TouchPhase.Began)
            {
                onMove = false;
                Vector3 aux;
                aux = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));

                points.Clear();
                if (actualLine) Destroy(actualLine); //Clear

                points.Add(transform.position);
                actualLine = Instantiate(line, points[0], transform.rotation);
                actualLine.GetComponent<LineRenderer>().positionCount++;
                actualLine.GetComponent<LineRenderer>().SetPositions(points.ToArray());
            }
            if (actualLine != null && points.Count < maxPoints)
            {
                Vector3 aux;
                aux = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
                if (IsPointCorrect(aux))
                {
                    onMove = true;
                    points.Add(new Vector3(aux.x, aux.y, -1));
                    actualLine.GetComponent<LineRenderer>().positionCount = points.Count;
                    actualLine.GetComponent<LineRenderer>().SetPositions(points.ToArray());
                }
            }
            if(swipe.phase == TouchPhase.Ended && points.Count == 1)
            {
                GameObject.Destroy(actualLine);
                onMove = false;
            }
        }
    }

    public void FollowLine()
    {
        if (actualLine.GetComponent<LineRenderer>().positionCount > 0 && points.Count > 0 && onMove)
        {
           transform.position = Vector3.MoveTowards(transform.position, points[0], Time.deltaTime * speed);
            
           if(Vector3.Distance(transform.position, points[0]) < dist ) //Maybe not necessary == could be ok
            {
                points.RemoveAt(0);
                actualLine.GetComponent<LineRenderer>().SetPositions(points.ToArray());
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Send position to the other player. Stream == Getter.
    {
        if (stream.IsWriting)
        {
            stream.SendNext(new Vector3(-transform.position.x, -transform.position.y, transform.position.z)); //Solo se envía si se está moviendo.
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
        }
    }

    private bool IsPointCorrect(Vector3 point)
    {
        if (Vector3.Distance(point, transform.position) < characterRad && points.Count == 1) return false;
        if (points.Count != 0)
        {
            if (Vector3.Distance(point, points[points.Count - 1]) < maxPointDist && Vector3.Distance(point, points[points.Count - 1]) > minPointDist) return true;
        }
        return false;
    }

    private Vector3 putZAxis(Vector3 vec) { return new Vector3(vec.x, vec.y, -1); }


}
