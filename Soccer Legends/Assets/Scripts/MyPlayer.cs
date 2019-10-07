using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;


public class MyPlayer : MonoBehaviourPun, IPunObservable
{
    public PhotonView pv;
    public float speed, dist, maxPointDist, minPointDist, characterRad, maxSize, shootTime;
    public GameObject playerCamera;
    public GameObject line;
    public int kick;

    public bool onMove = false;
    private Vector3 smoothMove, aux;
    private GameObject actualLine;
    private List<Vector3> points;
    private float touchTime;

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
            rePositionBall();
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
                touchTime = Time.time;
                aux = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
                if (Vector3.Distance(aux, transform.position) < characterRad)
                {
                    points.Clear();
                    onMove = false;
                    if (actualLine) Destroy(actualLine); //Clear

                    points.Add(transform.position);
                    actualLine = Instantiate(line, points[0], transform.rotation);
                    actualLine.GetComponent<LineRenderer>().positionCount++;
                    actualLine.GetComponent<LineRenderer>().SetPositions(points.ToArray());
                }
            }
            if (actualLine != null && TrailDistance() < maxSize)
            {
                aux = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
                if (IsPointCorrect(aux))
                {
                    onMove = true;
                    points.Add(putZAxis(aux));
                    actualLine.GetComponent<LineRenderer>().positionCount = points.Count;
                    actualLine.GetComponent<LineRenderer>().SetPositions(points.ToArray());
                }
            }
            if(swipe.phase == TouchPhase.Ended)
            {
                if (Time.time - touchTime <= shootTime)
                {
                    //Tap
                    if(transform.Find("Ball"))shootBall(aux);
                }
                if (points.Count == 1)
                {
                    GameObject.Destroy(actualLine);
                    onMove = false;
                }
            }
        }
    }

    public void FollowLine()
    {
        if (actualLine.GetComponent<LineRenderer>().positionCount > 0 && points.Count > 0 && onMove)
        {
           transform.position = Vector3.MoveTowards(transform.position, points[0], Time.deltaTime * speed);
            
           if(Vector3.Distance(transform.position, points[0]) < dist ) 
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

    private Vector3 putZAxis(Vector3 vec) { return new Vector3(vec.x, vec.y, 0); }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ball" && other.transform.parent == null)
        {
            other.transform.SetParent(transform);
            other.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            other.transform.position = transform.position;
        }
    }

    private void rePositionBall()
    {
        // 2 options of layer
    }
    private void shootBall(Vector3 tap)
    {
        Vector2 shootDir = new Vector2( tap.x - transform.Find("Ball").position.x, tap.y - transform.Find("Ball").position.y).normalized;
        transform.Find("Ball").GetComponent<Rigidbody2D>().AddForce(shootDir * kick, ForceMode2D.Impulse);
        transform.Find("Ball").SetParent(null);
    }

    private float TrailDistance()
    {
        float dist = 0;
        for (int i = 1; points.Count > i; i++) dist += Vector3.Distance(points[i], points[i - 1]);
        return dist;
    }
}
