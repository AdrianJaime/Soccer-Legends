﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;


public class IA_Player_PVE : MonoBehaviour
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
    public IA_manager.formationPositions ia_formationPos;
    public float speed, dist, maxPointDist, minPointDist, characterRad, maxSize, shootTime;
    public GameObject playerCamera, ball, line;
    public bool onMove = false, stunned = false, colliding = false;
    public string fightDir;
    public Vector3 playerObjective = Vector3.zero;
    public Vector2 startPosition = Vector2.zero;

    private Vector3 smoothMove, aux;
    private GameObject actualLine;
    private PVE_Manager mg;
    private List<Vector3> points;
    private Collider2D goal;
    private float touchTime;

    private void Start()
    {
        switch (gameObject.name.Substring(0, 5))
        {
            case "Pivot":
                ia_formationPos = IA_manager.formationPositions.PIVOT;
                break;
            case "Forwa":
                ia_formationPos = IA_manager.formationPositions.FORWARD;
                break;
            case "Goalk":
                ia_formationPos = IA_manager.formationPositions.GOALKEEPER;
                break;
            default:
                break;
        }
        stats = new Stats(0, 0, 0);
        int[] starr = { Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10) };
        fightDir = null;
        //if (photonView.IsMine)
        //{
        mg = GameObject.Find("Manager").GetComponent<PVE_Manager>();
        goal = GameObject.Find("Goal 1").GetComponent<Collider2D>();
        //}
        points = new List<Vector3>();
    }
    private void Update()
    {
        //if (photonView.IsMine) //Check if we're the local player
        //{
        if (startPosition == Vector2.zero) startPosition = transform.position;
        if (playerObjective != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerObjective, Time.deltaTime * speed);
            if (transform.position == playerObjective) MoveTo(new float[] { 0, 0, 0 });
        }
        rePositionBall();
        if (ball != null && goal.bounds.Contains(ball.transform.position) && gameObject.name != "GoalKeeper")
        {
            //Goal
            mg.Goal(false);
            //if(PhotonNetwork.IsMasterClient) mg.photonView.RPC("ChooseShoot", RpcTarget.AllViaServer, photonView.ViewID, findGoalKeeper().photonView.ViewID);
            //else mg.photonView.RPC("ChooseShoot", RpcTarget.AllViaServer, findGoalKeeper().photonView.ViewID, photonView.ViewID);
        }
    }

    private void smoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
    }



    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Send position to the other player. Stream == Getter.
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(new Vector3(-transform.position.x, -transform.position.y, transform.position.z)); //Solo se envía si se está moviendo.
    //        stream.SendNext(fightDir);
    //    }
    //    else if (stream.IsReading)
    //    {
    //        smoothMove = (Vector3)stream.ReceiveNext();
    //        fightDir = (string)stream.ReceiveNext();
    //    }
    //}

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
        if (other.tag == "Ball" && other.transform.parent == null && !stunned && mg.GameStarted)
        {
            GetBall();
            //photonView.RPC("GetBall", RpcTarget.AllViaServer);
        }

        //if (other.tag == "Player" && photonView.IsMine && !stunned && !other.GetComponent<MyPlayer>().stunned && !SameTeam(other.gameObject, gameObject))
        //{
        //    if (ball || other.GetComponent<MyPlayer>().ball)
        //    {
        //        fightDir = null;
        //        //mg.chooseDirection(gameObject.GetComponent<MyPlayer>(), other.GetComponent<MyPlayer>());
        //        if (PhotonNetwork.IsMasterClient) mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, gameObject.GetComponent<MyPlayer>().photonView.ViewID, other.GetComponent<MyPlayer>().photonView.ViewID);
        //        else mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, other.GetComponent<MyPlayer>().photonView.ViewID, gameObject.GetComponent<MyPlayer>().photonView.ViewID);
        //    }
        //}
    }

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.tag == "Player" && photonView.IsMine && !SameTeam(other.gameObject, gameObject))
    //    {
    //        if (ball || other.GetComponent<MyPlayer>().ball)
    //        {
    //            photonView.RPC("IsColliding", RpcTarget.AllViaServer, false);
    //        }
    //    }
    //}
    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    if (other.tag == "Player" && photonView.IsMine && !stunned && !other.GetComponent<MyPlayer>().stunned && !SameTeam(other.gameObject, gameObject))
    //    {
    //        if (ball || other.GetComponent<MyPlayer>().ball)
    //        {
    //            photonView.RPC("IsColliding", RpcTarget.AllViaServer, true);
    //        }
    //    }
    //}

    private MyPlayer findGoalKeeper()
    {
        GameObject[] GKarr = GameObject.FindGameObjectsWithTag("GoalKeeper");
        for (int i = 0; i < GKarr.Length; i++)
        {
            if (GKarr[i].transform.parent != transform.parent) return GKarr[i].GetComponent<MyPlayer>();
        }
        return null;
    }

    private void ShootBall(float[] _dir)
    {
        if (ball)
        {
            if (ball.GetComponent<Ball>().direction == Vector2.zero)
            {
                ball.GetComponent<Ball>().shootPosition = new Vector2(_dir[2], _dir[3]);
                ball.GetComponent<Ball>().direction = new Vector2(_dir[0], _dir[1]);
                //if(info.Sender.IsMasterClient)ball.GetComponent<Ball>().shooterIsMaster = true;
                //else ball.GetComponent<Ball>().shooterIsMaster = false;
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

    public void GetBall()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        ball.transform.parent = transform;
        ball.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }


    public void SetName(string name)
    {
        transform.GetChild(0).GetComponentInChildren<Text>().text = name;
    }

    public void IsColliding(bool isIt)
    {
        colliding = isIt;
        if (!isIt) playerObjective = Vector3.zero;
    }

    public void MoveTo(float[] objective)
    {
        playerObjective = new Vector3(objective[0], objective[1], objective[2]);
    }

    private bool SameTeam(GameObject P1, GameObject P2)
    {
        if (P1.transform.parent == P2.transform.parent) return true;
        else return false;
    }

    private void SetStats(int[] arr)
    {
        Debug.Log("Sent:" + arr[0] + arr[1] + arr[2]);
        stats = new Stats(arr[0], arr[1], arr[2]);
        //if(transform.GetChild(0).GetComponentInChildren<Text>().text != stats.shoot.ToString() + " " + stats.technique.ToString() + " " + stats.defense.ToString()) photonView.RPC("SetName", RpcTarget.AllViaServer, stats.shoot.ToString() + " " + stats.technique.ToString() + " " + stats.defense.ToString());
    }

    public void RepositionPlayer()
    {
        transform.position = startPosition;
        Lose();
        GameObject.Destroy(actualLine);
        onMove = false;
    }
}
