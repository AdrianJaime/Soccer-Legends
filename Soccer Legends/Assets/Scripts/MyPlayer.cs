﻿using System.Collections;
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
    public bool onMove = false, stunned = false, colliding = false;
    public string fightDir;
    public Vector3 playerObjective = Vector3.zero;
    public Vector2 startPosition = Vector2.zero;

    private Vector3 smoothMove, aux;
    private GameObject actualLine;
    private Manager mg;
    private List<Vector3> points;
    private Collider2D goal;
    private float touchTime;

    private void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 15;
        stats = new Stats(0, 0, 0);
        int[] starr = { Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10) };
        photonView.RPC("SetStats", RpcTarget.AllViaServer, starr);
        fightDir = null;
        if (photonView.IsMine)
        {
            if (gameObject.name != "GoalKeeper")
            {
                GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
                playerCamera.SetActive(true);
            }
            mg = GameObject.Find("Manager").GetComponent<Manager>();
            goal = GameObject.FindGameObjectWithTag("Goal").GetComponent<Collider2D>();
        }
        points = new List<Vector3>();
    }
    private void Update()
    {
        if (photonView.IsMine) //Check if we're the local player
        {
            if (startPosition == Vector2.zero) startPosition = transform.position;
            ProcessInputs();
            if (playerObjective != Vector3.zero) transform.position = Vector3.MoveTowards(transform.position, playerObjective, Time.deltaTime * 0.5f);
            else if (actualLine && points.Count > 1 && mg.GameOn && !stunned) FollowLine();
            rePositionBall(); //To be implemented
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
                if (Vector3.Distance(aux, transform.position) < characterRad && gameObject.name != "GoalKeeper")
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
            if (actualLine != null && TrailDistance() < maxSize && gameObject.name != "GoalKeeper")
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
                    
                    if (goal.bounds.Contains(aux) && gameObject.name != "GoalKeeper")
                    {
                        //Goal
                        if(PhotonNetwork.IsMasterClient) mg.photonView.RPC("ChooseShoot", RpcTarget.AllViaServer, photonView.ViewID, findGoalKeeper().photonView.ViewID);
                        else mg.photonView.RPC("ChooseShoot", RpcTarget.AllViaServer, findGoalKeeper().photonView.ViewID, photonView.ViewID);
                    }
                    else
                    {
                        //Pass
                        float[] dir = { aux.x, aux.y, transform.position.x, transform.position.y };
                        photonView.RPC("ShootBall", RpcTarget.AllViaServer, dir);
                    }
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
            //ball = other.gameObject;
            //ball.transform.SetParent(transform);
            //ball.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //ball.transform.position = transform.position;
            photonView.RPC("GetBall", RpcTarget.AllViaServer);
        }

        if(other.tag == "Player" && photonView.IsMine && !stunned && !other.GetComponent<MyPlayer>().stunned && !SameTeam(other.gameObject, gameObject))
        {
            if (ball || other.GetComponent<MyPlayer>().ball)
            {
                fightDir = null;
                //mg.chooseDirection(gameObject.GetComponent<MyPlayer>(), other.GetComponent<MyPlayer>());
                if (PhotonNetwork.IsMasterClient) mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, gameObject.GetComponent<MyPlayer>().photonView.ViewID, other.GetComponent<MyPlayer>().photonView.ViewID);
                else mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, other.GetComponent<MyPlayer>().photonView.ViewID, gameObject.GetComponent<MyPlayer>().photonView.ViewID);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && photonView.IsMine && !SameTeam(other.gameObject, gameObject))
        {
            if (ball || other.GetComponent<MyPlayer>().ball)
            {
                photonView.RPC("IsColliding", RpcTarget.AllViaServer, false);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && photonView.IsMine && !stunned && !other.GetComponent<MyPlayer>().stunned && !SameTeam(other.gameObject, gameObject))
        {
            if (ball || other.GetComponent<MyPlayer>().ball)
            {
                photonView.RPC("IsColliding", RpcTarget.AllViaServer, true);
            }
        }
    }

    private MyPlayer findGoalKeeper()
    {
        GameObject[] GKarr = GameObject.FindGameObjectsWithTag("GoalKeeper");
        for (int i = 0; i < GKarr.Length; i++)
        {
            if (GKarr[i].transform.parent != transform.parent) return GKarr[i].GetComponent<MyPlayer>();
        }
        return null;
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

    [PunRPC]
    public void Lose()
    {
        stunned = true;
        if (ball)
        {
            ball.transform.parent = null;
            ball = null;
        }
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

    [PunRPC]
    public void GetBall()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        ball.transform.parent = transform;
        ball.transform.localPosition = Vector3.zero;
        ball.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    [PunRPC]
    public void SetName(string name)
    {
        transform.GetChild(0).GetComponentInChildren<Text>().text = name;
    }

    [PunRPC]
    public void IsColliding(bool isIt)
    {
        colliding = isIt;
        if(!isIt)playerObjective = Vector3.zero;
    }

    [PunRPC]
    public void MoveTo(float[] objective)
    {
        playerObjective = new Vector3(objective[0], objective[1], objective[2]);
    }

    private bool SameTeam(GameObject P1, GameObject P2)
    {
        if (P1.transform.parent == P2.transform.parent) return true;
        else return false;
    }

    [PunRPC]
    private void SetStats(int[] arr)
    {
        Debug.Log("Sent:" + arr[0] + arr[1] + arr[2]);
        stats = new Stats(arr[0], arr[1], arr[2]);
        if(transform.GetChild(0).GetComponentInChildren<Text>().text != stats.shoot.ToString() + " " + stats.technique.ToString() + " " + stats.defense.ToString()) photonView.RPC("SetName", RpcTarget.AllViaServer, stats.shoot.ToString() + " " + stats.technique.ToString() + " " + stats.defense.ToString());
    }

    public void RepositionPlayer()
    {
        transform.position = startPosition;
        Lose();
        GameObject.Destroy(actualLine);
        onMove = false;
    }
}
