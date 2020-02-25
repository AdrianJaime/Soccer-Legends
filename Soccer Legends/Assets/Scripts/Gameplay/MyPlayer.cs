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
    public float speed, dist, maxPointDist, minPointDist, characterRad, maxSize, shootTime;
    public GameObject ball, line;
    public bool onMove = false, stunned = false, colliding = false, covered = false;
    public string fightDir;
    public Vector3 playerObjective = Vector3.zero;
    public Vector2 startPosition = Vector2.zero;

    public IA_manager.formationPositions formationPos;

    PhotonView pv;

    private Vector3 smoothMove, aux;
    private GameObject actualLine;
    private Manager mg;
    private List<Vector3> points;
    public Collider2D goal, rival_goal;
    private float touchTime;
    private int shootFramesRef;
    private int passFrames = 0;
    public int fingerIdx = -1;

    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer characterSprite;
    float velocity0 = 0;


    private void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 15;
        Debug.Log(photonView.isActiveAndEnabled);
        switch (gameObject.transform.GetSiblingIndex())
        {
            case 0:
                formationPos = IA_manager.formationPositions.CIERRE;
                gameObject.name = "Cierre";
                break;
            case 1:
                formationPos = IA_manager.formationPositions.ALA;
                gameObject.name = "Ala";
                break;
            case 2:
                formationPos = IA_manager.formationPositions.PIVOT;
                gameObject.name = "Pivot";
                break;
            case 3:
                formationPos = IA_manager.formationPositions.GOALKEEPER;
                break;
            default:

                break;
        }
        SetName(gameObject.name);
        if (!photonView.IsMine) stats = new Stats(5, 3, 3);
        else
        {
            stats = new Stats(7, 5, 5);
        }

        //int[] starr = { Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10) };
        //photonView.RPC("SetStats", RpcTarget.AllViaServer, starr);
        int[] starr = { Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10) };
        fightDir = null;
        //if (photonView.IsMine)
        //{
        mg = GameObject.Find("Manager").GetComponent<Manager>();
        if (!photonView.IsMine)
        {
            goal = GameObject.Find("Goal 1").GetComponent<Collider2D>();
            rival_goal = GameObject.Find("Goal 2").GetComponent<Collider2D>();
        }
        else
        {
            goal = GameObject.Find("Goal 2").GetComponent<Collider2D>();
            rival_goal = GameObject.Find("Goal 1").GetComponent<Collider2D>();
        }
        //}
        points = new List<Vector3>();
        shootFramesRef = Time.frameCount - 5;
    }
    private void Update()
    {
        if (mg.GameOn)
        {
            if (photonView.IsMine) //Check if we're the local player
            {
                if (!stunned)
                {
                    if (formationPos == IA_manager.formationPositions.GOALKEEPER) MoveTo(new float[] { GameObject.FindGameObjectWithTag("Ball").transform.position.x, transform.position.y, 0.0f });
                    if (startPosition == Vector2.zero) startPosition = transform.position;
                    if (photonView.IsMine && ball != null) ProcessInputs();
                    if (playerObjective != Vector3.zero)
                    {
                        //Vector3 nextPos;
                        Vector3 newPos = Vector3.MoveTowards(transform.position, playerObjective, Time.deltaTime * speed);
                        velocity0 = (newPos - transform.position).magnitude;
                        transform.position = newPos;
                        //nextPos = Vector3.MoveTowards(transform.position, playerObjective, Time.deltaTime * speed);
                        //GetComponent<Rigidbody2D>().velocity = (nextPos - transform.position).normalized;
                        if (transform.position == playerObjective) MoveTo(new float[] { 0, 0, 0 });
                    }
                    else GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            }
            else if (!photonView.IsMine)
            {
                smoothMovement();
            }
            if (GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.IsMine)
            {
                checkCollisionDetection();
                //rePositionBall(); //To be implemented
                checkGoal();
            }
        }
        else GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        SetAnimatorValues();
    }

    private void smoothMovement()
    {
        if (Vector2.Distance(smoothMove, transform.position) < 0.75f)
        {
            //Vector3 nextPos;
            Vector3 newPos = Vector3.MoveTowards(transform.position, smoothMove, Time.deltaTime * speed);
            velocity0 = (newPos - transform.position).magnitude;
            transform.position = newPos;
            //nextPos = Vector3.MoveTowards(transform.position, playerObjective, Time.deltaTime * speed);
            //GetComponent<Rigidbody2D>().velocity = (nextPos - transform.position).normalized;
            if (transform.position == playerObjective) MoveTo(new float[] { 0, 0, 0 });
        }
        else
        {
            Vector3 newPos = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
            velocity0 = (newPos - transform.position).magnitude;
            transform.position = newPos;
        }
    }

    private void ProcessInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float[] dir = { 0, 0, ball.transform.position.x, ball.transform.position.y };
            photonView.RPC("ShootBall", RpcTarget.AllViaServer, dir);
        }
        //Movement
        if ((Input.touchCount > mg.getTotalTouches() || fingerIdx != -1))
        {
            if (fingerIdx == -1) fingerIdx = mg.getTouchIdx();
            else if (fingerIdx == -1) return;
            Touch swipe = Input.GetTouch(fingerIdx);
            aux = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
            if (swipe.phase == TouchPhase.Began)
            {
                touchTime = Time.time;
                aux = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
            }
            else if (swipe.phase == TouchPhase.Moved)
            {
                mg.releaseTouchIdx(fingerIdx);
                fingerIdx = -1;
            }
            if (swipe.phase == TouchPhase.Ended)
            {
                if (Time.time - touchTime <= shootTime * 2 && mg.GameOn && !stunned)
                {

                    if (ball != null)
                    {
                        //Pass
                        float[] dir = { aux.x, aux.y, ball.transform.position.x, ball.transform.position.y };
                        photonView.RPC("ShootBall", RpcTarget.AllViaServer, dir);
                    }
                }
                mg.releaseTouchIdx(fingerIdx);
                fingerIdx = -1;
                passFrames = 0;
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
            stream.SendNext(stunned);
            //stream.SendNext(photonView.ViewID);
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
            fightDir = (string)stream.ReceiveNext();
            stunned = (bool)stream.ReceiveNext();
            //photonView.ViewID = (int)stream.ReceiveNext();
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
                PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).GetComponent<Ball>().shootPosition = new Vector2(_dir[2], _dir[3]);
                PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).GetComponent<Ball>().direction = new Vector2(_dir[0], _dir[1]);
                if(info.Sender.IsMasterClient) PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).GetComponent<Ball>().shooterIsMaster = true;
                else PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).GetComponent<Ball>().shooterIsMaster = false;
                //ball.GetComponent<Ball>().shoot = true;
                shootFramesRef = Time.frameCount;

                PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).transform.parent = null;
                ball = null;
                transform.gameObject.layer = 0;
                mg.lastPlayer = gameObject;
                PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).GetComponent<Ball>().photonView.RPC("ShootBall", RpcTarget.AllViaServer, new float[] { _dir[0], _dir[1] });
            }
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
        Debug.Log(gameObject.name + " from " + gameObject.transform.parent.name + " lost the Fight");
        stunned = true;
        mg.photonView.RPC("resumeGame", RpcTarget.AllViaServer);
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
            characterSprite.enabled = false;
            yield return new WaitForSeconds(0.2f);
            characterSprite.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
        stunned = false;
    }

    [PunRPC]
    public void GetBall()
    {
        if (PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).transform.parent != null)
        {
            return;
        }
        ball = PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).gameObject;
        ball.transform.parent = transform;
        ball.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ball.GetComponent<Ball>().photonView.TransferOwnership(photonView.Owner);
        transform.gameObject.layer = 2;
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
        if (formationPos == IA_manager.formationPositions.GOALKEEPER && Mathf.Abs(objective[0]) > 1.25f)
            objective[0] = (objective[0] / Mathf.Abs(objective[0])) * 1.25f;
        playerObjective =  new Vector3(objective[0], objective[1], objective[2]);
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
        transform.parent.GetComponent<IA_manager>().check_IA_Shoot();
    }

    public void stablishNewShootCheck()
    {
        StartCoroutine(checkIA_Shoot_After_Time(Random.Range(1.0f, 3.0f)));
    }



    void checkCollisionDetection()
    {
        float detectionDist = 0.5f;
        GameObject[] rivals;
        bool foundCovered = false;

        rivals = mg.myIA_Players;
        foreach (GameObject rival in rivals)
        {
            if (Vector2.Distance(rival.transform.position, transform.position) < detectionDist + 0.1 && !rival.GetComponent<MyPlayer>().stunned)
            {
                if (ball != null && rival.GetComponent<MyPlayer>().ball == null)
                {
                    int ia_Idx = 0;
                    int playerIdx = 0;
                    fightDir = null;
                    for (int i = 0; i < mg.myPlayers.Length; i++)
                    {
                        if (!photonView.IsMine)
                        {
                            if (gameObject == mg.myIA_Players[i]) ia_Idx = i;
                            if (rival.gameObject == mg.myPlayers[i]) playerIdx = i;
                        }
                        else
                        {
                            if (rival.gameObject == mg.myIA_Players[i]) ia_Idx = i;
                            if (gameObject == mg.myPlayers[i]) playerIdx = i;
                        }
                    }
                    //mg.chooseDirection(playerIdx, ia_Idx); // LLAMAR COMO RPC
                    mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, gameObject.GetComponent<MyPlayer>().photonView.ViewID, rival.GetComponent<MyPlayer>().photonView.ViewID);
                    //else photonView.RPC("chooseDirection", RpcTarget.AllViaServer, rival.GetComponent<MyPlayer>().photonView.ViewID, gameObject.GetComponent<MyPlayer>().photonView.ViewID);
                    return;
                    //mg.chooseDirection(gameObject.GetComponent<MyPlayer>(), other.GetComponent<MyPlayer>());
                }
                else if (ball == null) foundCovered = covered = true;
            }
            else if (!foundCovered) covered = false;
        }

        if (PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).transform.parent == null && Vector2.Distance(GameObject.FindGameObjectWithTag("Ball").transform.position, transform.position - new Vector3(0, 0.5f, 0)) < detectionDist && !stunned && mg.GameStarted && shootFramesRef + 100 < Time.frameCount)
        {
            photonView.RPC("GetBall", RpcTarget.AllViaServer);
        }
        else if(PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).transform.parent != transform) ball = null;

    }

    void checkGoal()
    {
        Debug.Log("checkGoal");
        if (mg.GameOn && mg.lastPlayer == gameObject)
        {
            if (goal.bounds.Contains(PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).transform.position)
                && PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).Owner == photonView.Owner)
            {
                //Goal
                int ia_Idx = 3;
                //int playerIdx = 0;
                //fightDir = null;
                //if (!!photonView.IsMine)
                //{
                //    for (int i = 0; i < mg.myPlayers.Length; i++)
                //    {

                //        if (gameObject == mg.myPlayers[i])
                //        {
                //            playerIdx = i;
                //            break;
                //        }
                //    }
                //}
                //else
                //{
                //    playerIdx = 3;
                //    for (int i = 0; i < mg.myIA_Players.Length; i++)
                //    {

                //        if (gameObject == mg.myIA_Players[i])
                //        {
                //            ia_Idx = i;
                //            break;
                //        }
                //    }
                //}
                mg.photonView.RPC("ChooseShoot", RpcTarget.AllViaServer, mg.lastPlayer.GetComponent<MyPlayer>().photonView.ViewID, mg.myIA_Players[ia_Idx].GetComponent<MyPlayer>().photonView.ViewID);
                //if(PhotonNetwork.IsMasterClient) mg.photonView.RPC("ChooseShoot", RpcTarget.AllViaServer, photonView.ViewID, findGoalKeeper().photonView.ViewID);
                //else mg.photonView.RPC("ChooseShoot", RpcTarget.AllViaServer, findGoalKeeper().photonView.ViewID, photonView.ViewID);
            }
        }
        else return;
    }

    void SetAnimatorValues()
    {
        Vector2 direction = Vector3.zero;
        if (photonView.IsMine)
        {
            direction = (playerObjective - transform.position).normalized;
        }
        else
        {
            direction = (smoothMove - transform.position).normalized;
        }
        animator.SetFloat("DirectionX", direction.x);
        animator.SetFloat("DirectionY", direction.y);
        if (velocity0 < 0.01)
            animator.SetBool("Moving", false);
        else
            animator.SetBool("Moving", true);

        if (mg.GameOn != animator.enabled)
            animator.enabled = mg.GameOn;
    }
}
