using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;


public class MyPlayer : MonoBehaviourPun, IPunObservable
{

    public class Stats
    {
        public int shoot, technique, defense;

        public Stats(int _shoot, int _technique, int _defense)
        {
            shoot = _shoot;
            technique = _technique;
            defense = _defense;
        }
    }

    public Stats stats;
    public PhotonView pv;
    public float speed, dist, maxPointDist, minPointDist, characterRad, maxSize, shootTime;
    public GameObject playerCamera, ball, line;
    public bool onMove = false;
    public string fightDir;

    private Vector3 smoothMove, aux;
    private GameObject actualLine;
    private Manager mg;
    private List<Vector3> points;
    private float touchTime;
    private bool stunned = false;

    private void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 15;
        fightDir = null;
        stats = new Stats(Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10));
        if (photonView.IsMine)
        {
            GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
            playerCamera.SetActive(true);
            mg = GameObject.Find("Manager").GetComponent<Manager>();
        }
        points = new List<Vector3>();
    }
    private void Update()
    {
        if (photonView.IsMine) //Check if we're the local player
        {
            ProcessInputs();
            if (actualLine && points.Count > 1 && mg.GameOn && !stunned) FollowLine();
            rePositionBall(); //To be implemented
            transform.GetChild(0).GetChild(0).GetComponent<Text>().text = fightDir;
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
                if (Time.time - touchTime <= shootTime && mg.GameOn && !stunned)
                {
                    //Tap
                    float[] dir = { aux.x, aux.y, transform.position.x, transform.position.y };
                    photonView.RPC("ShootBall", RpcTarget.AllViaServer, dir);
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
            stream.SendNext(fightDir);
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
            fightDir = (string)stream.ReceiveNext();
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
        if(other.tag == "Ball" && other.transform.parent == null && !stunned && GameObject.Find("Manager").GetComponent<Manager>().GameStarted)
        {
            ball = other.gameObject;
            ball.transform.SetParent(transform);
            ball.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            ball.transform.position = transform.position;
        }

        if(other.tag == "Player" && photonView.IsMine && !stunned)
        {
            if (ball || other.GetComponent<MyPlayer>().ball)
            {
                fightDir = null;
                mg.chooseDirection(gameObject.GetComponent<MyPlayer>(), other.GetComponent<MyPlayer>());
            }
        }
    }

    [PunRPC]
    private void ShootBall(float[] _dir, PhotonMessageInfo info)
    {
        if (ball)
        {
            if (ball.GetComponent<Ball>().direction == Vector2.zero)
            {
                ball.GetComponent<Ball>().shootPosition = new Vector2(_dir[2], _dir[3]);
                ball.GetComponent<Ball>().direction = new Vector2(_dir[0], _dir[1]);
                if(info.Sender.IsMasterClient)ball.GetComponent<Ball>().shooterIsMaster = true;
                else ball.GetComponent<Ball>().shooterIsMaster = false;
                ball.GetComponent<Ball>().shoot = true;
            }
            ball.transform.parent = null;
            ball = null;
        }
        else Debug.Log("No");
    }

    private void rePositionBall()
    {
        // 2 options of layer
    }

    private float TrailDistance()
    {
        float dist = 0;
        for (int i = 1; points.Count > i; i++) dist += Vector3.Distance(points[i], points[i - 1]);
        return dist;
    }

    public void Lose()
    {
        stunned = true;
        StartCoroutine(Blink(2.0f));
    }

    private IEnumerator Blink(float waitTime)
    {
        var endTime = Time.time + waitTime;
        while (Time.time < endTime)
        {
            GetComponent<Renderer>().enabled = false;
            yield return new WaitForSeconds(0.2f);
            GetComponent<Renderer>().enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
        stunned = false;
    }
}
