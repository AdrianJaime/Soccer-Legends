using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;


public class MyPlayer_PVE : MonoBehaviour
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
    public float speed, dist, maxPointDist, minPointDist, characterRad, maxSize, shootTime;
    public GameObject playerCamera, ball, line;
    public bool onMove = false, stunned = false, colliding = false, covered = false;
    public string fightDir;
    public Vector3 playerObjective = Vector3.zero;
    public Vector2 startPosition = Vector2.zero;

    public IA_manager.formationPositions formationPos;

    private Vector3 smoothMove, aux;
    private GameObject actualLine;
    private PVE_Manager mg;
    private List<Vector3> points;
    private Collider2D goal;
    private float touchTime;
    private int shootFramesRef;

    //IA
    bool iaPlayer;

    private void Start()
    {
        if (transform.parent.name.Substring(0, 7) == "Team IA") iaPlayer = true;
        else iaPlayer = false;
        switch (gameObject.name.Substring(0, 5))
        {
            case "Pivot":
                formationPos = IA_manager.formationPositions.PIVOT;
                break;
            case "Forwa":
                formationPos = IA_manager.formationPositions.FORWARD;
                break;
            case "Goalk":
                formationPos = IA_manager.formationPositions.GOALKEEPER;
                break;
            default:
                break;
        }
        stats = new Stats(Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10));
        int[] starr = { Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10) };
        fightDir = null;
        //if (photonView.IsMine)
        //{
            if (!iaPlayer && gameObject.name != "GoalKeeper")
            {
                //GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
                playerCamera.SetActive(false);
            }
            mg = GameObject.Find("Manager").GetComponent<PVE_Manager>();
            if (iaPlayer) goal = GameObject.Find("Goal 1").GetComponent<Collider2D>();
            else goal = GameObject.FindGameObjectWithTag("Goal").GetComponent<Collider2D>();
        //}
        points = new List<Vector3>();
        shootFramesRef = Time.frameCount - 5;
    }
    private void Update()
    {
        //if (photonView.IsMine) //Check if we're the local player
        //{
        if (mg.GameOn && !stunned)
        {
            if (startPosition == Vector2.zero) startPosition = transform.position;
            if (!iaPlayer) ProcessInputs();
            if (points.Count > 1 && mg.GameOn && !stunned) FollowLine();
            else if (playerObjective != Vector3.zero && (playerCamera == null || !playerCamera.activeSelf))
            {
                transform.position = Vector3.MoveTowards(transform.position, playerObjective, Time.deltaTime * speed);
                if (transform.position == playerObjective) MoveTo(new float[] { 0, 0, 0 });
            }

            if (iaPlayer && ball != null && goal.bounds.Contains(ball.transform.position))
            {
                int ia_Idx = 0;
                int playerIdx = 3;
                fightDir = null;
                for (int i = 0; i < mg.myPlayers.Length; i++)
                {
                        if (gameObject == mg.myIA_Players[i])
                        {
                            ia_Idx = i;
                            break;
                        }
                }
                mg.ChooseShoot(playerIdx, ia_Idx);
            }

            rePositionBall(); //To be implemented
        }
        //}
        //else if(!photonView.IsMine)
        //{
           // smoothMovement();
        //}

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
                    MoveTo(new float[] { 0, 0, 0 }); //Clear from other objectives
                    if (actualLine) Destroy(actualLine); //Clear line

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
                        int ia_Idx = 3;
                        int playerIdx = 0;
                        fightDir = null;
                        for (int i = 0; i < mg.myPlayers.Length; i++)
                        {

                            if (gameObject == mg.myPlayers[i])
                            {
                                playerIdx = i;
                                break;
                            }
                        }                        
                        mg.ChooseShoot(playerIdx, ia_Idx);
                        //if(PhotonNetwork.IsMasterClient) mg.photonView.RPC("ChooseShoot", RpcTarget.AllViaServer, photonView.ViewID, findGoalKeeper().photonView.ViewID);
                        //else mg.photonView.RPC("ChooseShoot", RpcTarget.AllViaServer, findGoalKeeper().photonView.ViewID, photonView.ViewID);
                    }
                    else
                    {
                        //Pass
                        float[] dir = { aux.x, aux.y, transform.position.x, transform.position.y };
                        ShootBall(dir);
                        //photonView.RPC("ShootBall", RpcTarget.AllViaServer, dir);
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
           transform.position = Vector3.MoveTowards(transform.position, points[0], Time.deltaTime * speed * 2);
            
           if(Vector3.Distance(transform.position, points[0]) < dist ) 
            {
                points.RemoveAt(0);
                actualLine.GetComponent<LineRenderer>().SetPositions(points.ToArray());
            }
        }
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
        if (other.tag == "Ball" && other.transform.parent == null && !stunned && mg.GameStarted && shootFramesRef + 5 < Time.frameCount)
        {
            GetBall();
            //photonView.RPC("GetBall", RpcTarget.AllViaServer);
        }

        if (other.transform.GetComponent<MyPlayer_PVE>() != null && other.transform.parent.name.Substring(0, 7) != "Team IA" && Math.Abs(other.transform.position.y - transform.position.y) < 0.25f)
        {
            covered = true;
            if (ball) check_IA_Shoot();
        }
        //if (other.tag == "Player" && !stunned && !other.GetComponent<MyPlayer_PVE>().stunned && other.transform.parent != transform.parent)
        //{
        //    if (ball != null || other.GetComponent<MyPlayer_PVE>().ball != null)
        //    {
        //        int ia_Idx = 0;
        //        int playerIdx = 0;
        //        fightDir = null;
        //        for (int i = 0; i < mg.myPlayers.Length; i++)
        //        {
        //            if(iaPlayer)
        //            {
        //                if (gameObject == mg.myIA_Players[i]) ia_Idx = i;
        //                if (other.gameObject == mg.myPlayers[i]) playerIdx = i;
        //            }
        //            else
        //            {
        //                if (other.gameObject == mg.myIA_Players[i]) ia_Idx = i;
        //                if (gameObject == mg.myPlayers[i]) playerIdx = i;
        //            }
        //        }
        //        Debug.Log("Player-> " + playerIdx.ToString());
        //        Debug.Log("IA-> " + ia_Idx.ToString());
        //        mg.chooseDirection(playerIdx, ia_Idx);
        //        //mg.chooseDirection(gameObject.GetComponent<MyPlayer>(), other.GetComponent<MyPlayer>());
        //        //if (PhotonNetwork.IsMasterClient) mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, gameObject.GetComponent<MyPlayer>().photonView.ViewID, other.GetComponent<MyPlayer>().photonView.ViewID);
        //        //else mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, other.GetComponent<MyPlayer>().photonView.ViewID, gameObject.GetComponent<MyPlayer>().photonView.ViewID);
        //    }
        //}
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //if (other.tag == "Player" && photonView.IsMine && !SameTeam(other.gameObject, gameObject))
        //{
        //    if (ball || other.GetComponent<MyPlayer>().ball)
        //    {
        //        photonView.RPC("IsColliding", RpcTarget.AllViaServer, false);
        //    }
        //}
        if (other.tag == "Ball" && other.transform.parent == null && !stunned && mg.GameStarted && shootFramesRef + 5 < Time.frameCount)
        {
            GetBall();
            //photonView.RPC("GetBall", RpcTarget.AllViaServer);
        }

        if (other.transform.GetComponent<MyPlayer_PVE>() != null && other.transform.parent.name.Substring(0, 7) != "Team IA")
        {
            covered = false;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.transform.GetComponent<MyPlayer_PVE>() != null && other.transform.parent.name.Substring(0, 7) != "Team IA" && Math.Abs(other.transform.position.y - transform.position.y) < 0.25f)
        {
            covered = true;
        }
        else if (other.transform.GetComponent<MyPlayer_PVE>() != null && other.transform.parent.name.Substring(0, 7) != "Team IA" && Math.Abs(other.transform.position.y - transform.position.y) >= 0.25f)
        {
            covered = false;
        }
        if (other.tag == "Player" && !stunned && !other.GetComponent<MyPlayer_PVE>().stunned && other.transform.parent != transform.parent && Math.Abs(other.transform.position.y - transform.position.y) < 0.25f)
        {
            if (ball != null || other.GetComponent<MyPlayer_PVE>().ball != null)
            {
                int ia_Idx = 0;
                int playerIdx = 0;
                fightDir = null;
                for (int i = 0; i < mg.myPlayers.Length; i++)
                {
                    if (iaPlayer)
                    {
                        if (gameObject == mg.myIA_Players[i]) ia_Idx = i;
                        if (other.gameObject == mg.myPlayers[i]) playerIdx = i;
                    }
                    else
                    {
                        if (other.gameObject == mg.myIA_Players[i]) ia_Idx = i;
                        if (gameObject == mg.myPlayers[i]) playerIdx = i;
                    }
                }
                mg.chooseDirection(playerIdx, ia_Idx);
                //mg.chooseDirection(gameObject.GetComponent<MyPlayer>(), other.GetComponent<MyPlayer>());
                //if (PhotonNetwork.IsMasterClient) mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, gameObject.GetComponent<MyPlayer>().photonView.ViewID, other.GetComponent<MyPlayer>().photonView.ViewID);
                //else mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, other.GetComponent<MyPlayer>().photonView.ViewID, gameObject.GetComponent<MyPlayer>().photonView.ViewID);
            }
        }
        //if (other.tag == "Player" && photonView.IsMine && !stunned && !other.GetComponent<MyPlayer>().stunned && !SameTeam(other.gameObject, gameObject))
        //{
        //    if (ball || other.GetComponent<MyPlayer>().ball)
        //    {
        //        photonView.RPC("IsColliding", RpcTarget.AllViaServer, true);
        //    }
        //}
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
                ball.GetComponent<Ball>().shooterIsMaster = false;
                ball.GetComponent<Ball>().ShootBall(_dir);
                shootFramesRef = Time.frameCount;
            }
            //if (!iaPlayer && (GameObject.FindGameObjectWithTag("MainCamera") != null)) {
            //    GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
            //    }
            ball.transform.parent = null;
            ball = null;
            transform.gameObject.layer = 0;
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
        Debug.Log(gameObject.name + " from " + gameObject.transform.parent.name + " lost the Fight");
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
        if(ball.transform.localPosition.y > 0)
        {
            ball.transform.parent = null;
            ball = null;
            return;
        }
        ball.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.gameObject.layer = 2;

        if(iaPlayer)stablishNewShootCheck();
        //if (!iaPlayer && (GameObject.FindGameObjectWithTag("MainCamera") != null))
        //{
        //    GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
        //    playerCamera.SetActive(true);
        //}
    }


    public void SetName(string name)
    {
        transform.GetChild(0).GetComponentInChildren<Text>().text = name;
    }

    public void IsColliding(bool isIt)
    {
        colliding = isIt;
        if(!isIt)playerObjective = Vector3.zero;
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
        transform.gameObject.layer = 0;
        transform.position = startPosition;
        Lose();
        GameObject.Destroy(actualLine);
        onMove = false;
    }

    IEnumerator checkIA_Shoot_After_Time(float time)
    {
        yield return new WaitForSeconds(time);

        if (!ball) yield break;

        check_IA_Shoot();
    }

    void stablishNewShootCheck()
    {
        StartCoroutine(checkIA_Shoot_After_Time(Random.Range(0.1f, 1.0f)));
    }

    void check_IA_Shoot()
    {
        Vector3 shootingTarget;
        List<Vector3> closePlayers = new List<Vector3>();

        for (int i = 0; i < mg.myIA_Players.Length; i++)
        {
            if (Vector2.Distance(transform.position, mg.myIA_Players[i].transform.position) < 9 && mg.myIA_Players[i] != gameObject) closePlayers.Add(mg.myIA_Players[i].transform.position);
        }
        while (closePlayers.Count != 0)
        {
            shootingTarget = closePlayers[0];
            float minDist = Vector2.Distance(shootingTarget, transform.position);
            if (closePlayers.Count > 1)
            {
                for (int i = 0; i < closePlayers.Count; i++)
                {
                    Debug.Log("Player " + (i + 1).ToString() + "-> " + Vector2.Distance(transform.position, closePlayers[i]).ToString());
                    if (Vector2.Distance(transform.position, closePlayers[i]) < minDist)
                    {
                        Debug.Log("Target change to-> " + (i + 1).ToString());
                        shootingTarget = closePlayers[i];
                        minDist = Vector2.Distance(transform.position, closePlayers[i]);
                    }
                }
            }
            RaycastHit2D hit;
            Vector3 dir = shootingTarget - ball.transform.position;
            Debug.Log("Final distance-> " + Vector2.Distance(transform.position, shootingTarget).ToString());
            hit = Physics2D.Raycast(ball.transform.position, dir, 10);
            if (hit && hit.transform.parent == transform.parent && hit.transform.name != transform.name && !hit.transform.GetComponent<MyPlayer_PVE>().covered)
            {
                Debug.Log(hit.transform.name);
                Debug.Log("Shoot distance-> " + Vector2.Distance(transform.position, hit.transform.position).ToString());
                ShootBall(new float[] { hit.transform.position.x, hit.transform.position.y, transform.position.x, transform.position.y });
                closePlayers.Clear();
            }
            else
            {
                Debug.Log("Unable to pass to player with distance-> " + Vector2.Distance(transform.position, hit.transform.position).ToString());
                closePlayers.Remove(shootingTarget);
            }
        }

        if (ball != null) stablishNewShootCheck();
    }
}
