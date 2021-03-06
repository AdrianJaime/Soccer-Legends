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
        public Stats(Stats _stats)
        {
            shoot = _stats.shoot;
            technique = _stats.technique;
            defense = _stats.defense;
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

    public CharacterBasic characterBasic;

    private Vector3 smoothMove, aux;
    public Vector3 realOnlinePosition { get { return smoothMove; } }
    private GameObject actualLine;
    private Manager mg;
    private List<Vector3> points;
    public Collider2D goal, rival_goal;
    private float touchTime;
    private int passFrames = 0;
    public int fingerIdx = -1;

    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer characterSprite;
    public Sprite confrontationSprite;
    public Sprite specialSprite;

    PVP_strategyUI strategyScript;

    float goalKeeperRef;

    Vector2 animDir;
    float velocity0 = 0;


    private void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 20;
        Debug.Log(photonView.isActiveAndEnabled);
        mg = GameObject.Find("Manager").GetComponent<Manager>();
        strategyScript = GameObject.Find("StrategiesAndEnergy uP").GetComponent<PVP_strategyUI>();
        setPlayer();

        //int[] starr = { Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10) };
        //photonView.RPC("SetStats", RpcTarget.AllViaServer, starr);
        int[] starr = { Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10) };
        fightDir = null;
        //if (photonView.IsMine)
        //{
        
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
        StartCoroutine(SetAnimatorValues());
    }

    void setPlayer()
    {
        List<CharacterBasic> teamInfo = !photonView.IsMine ? StaticInfo.rivalTeam : StaticInfo.teamSelectedToPlay;
        switch (gameObject.transform.GetSiblingIndex())
        {
            case 0:
                formationPos = IA_manager.formationPositions.CIERRE;
                gameObject.name = "Cierre";
                characterBasic = teamInfo[2];
                break;
            case 1:
                formationPos = IA_manager.formationPositions.ALA;
                gameObject.name = "Ala";
                characterBasic = teamInfo[0];
                break;
            case 2:
                formationPos = IA_manager.formationPositions.PIVOT;
                gameObject.name = "Pivot";
                characterBasic = teamInfo[1];
                break;
            case 3:
                formationPos = IA_manager.formationPositions.GOALKEEPER;
                characterBasic = teamInfo[3];
                speed *= 3;
                break;
            default:
                break;
        }

        SetStats();
        confrontationSprite = characterBasic.basicInfo.artworkConforntation;
        specialSprite = characterBasic.basicInfo.completeArtwork;
        characterBasic.basicInfo.specialAttackInfo.LoadSpecialAtack();
        animator.runtimeAnimatorController = characterBasic.basicInfo.animator_character;
        SetName(gameObject.name);
    }

    private void Update()
    {
        if (mg.GameOn)
        {
            if (photonView.IsMine) //Check if we're the local player
            {
                if (!stunned)
                {
                    if (formationPos == IA_manager.formationPositions.GOALKEEPER)
                    {
                        MoveTo(new float[] { GameObject.FindGameObjectWithTag("Ball").transform.position.x, transform.position.y, 0.0f });
                        if (ball != null && goalKeeperRef + 5.0f < Time.time)
                        {
                            Vector2 randShoot = new Vector2(Random.Range(-10.0f, 10.0f), Random.Range(1.0f, 5.0f));
                            photonView.RPC("ShootBall", RpcTarget.AllViaServer, new float[] { randShoot.x, randShoot.y, transform.position.x, transform.position.y });
                        }
                    }
                    if (startPosition == Vector2.zero) startPosition = transform.position;
                    if (photonView.IsMine && ball != null) ProcessInputs();
                    if (playerObjective != Vector3.zero)
                    {
                        //Vector3 nextPos;
                        float runSpeed = speed;
                        if (mg.HasTheBall() == 2) runSpeed += speed * 0.25f;
                        Vector3 newPos = Vector3.MoveTowards(transform.position, playerObjective, Time.deltaTime * runSpeed);
                        GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude((Vector2)(newPos - transform.position) + GetComponent<Rigidbody2D>().velocity, Time.deltaTime * speed);
                        velocity0 = ((Vector2)(newPos - transform.position) + GetComponent<Rigidbody2D>().velocity * Time.deltaTime).magnitude;
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
            }
            else if (PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).transform.parent == transform) ball = PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).gameObject;
            else if (PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).transform.parent != transform) ball = null;

        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            if (!photonView.IsMine) transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 100.0f);
        }

    }

    private void smoothMovement()
    {
        smoothMove = new Vector3(smoothMove.x, smoothMove.y, transform.position.y / 100.0f);
        if (Vector2.Distance(smoothMove, transform.position) < 0.75f)
        {
            //Vector3 nextPos;
            Vector3 newPos = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 7.5f);
            velocity0 = ((Vector2)(newPos - transform.position) + GetComponent<Rigidbody2D>().velocity * Time.deltaTime).magnitude;
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
        //Movement
        if ((Input.touchCount > mg.getTotalTouches() || fingerIdx != -1))
        {
            if (fingerIdx == -1) fingerIdx = mg.getTouchIdx();
            Touch swipe;
            if (Input.touchCount > fingerIdx) swipe = Input.GetTouch(fingerIdx);
            else
            {
                mg.releaseTouchIdx(fingerIdx);
                fingerIdx = -1;
                return;
            }
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
                if (Time.time - touchTime <= shootTime * 2 && mg.GameOn && !stunned && !strategyScript.isInteracting())
                {

                    if (ball != null)
                    {
                        if (goal.bounds.Contains(aux) && ball.GetComponent<Ball>().inArea &&
                            mg.fightRef + 1.0f <= Time.time) checkGoal();
                        else
                        {
                            //Pass
                            float[] dir = { aux.x, aux.y, ball.transform.position.x, ball.transform.position.y };
                            photonView.RPC("ShootBall", RpcTarget.All, dir);
                        }
                        Instantiate(mg.circleTapPrefab, aux, mg.circleTapPrefab.transform.rotation, null);
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
            float y_offset = 1.0f;
            stream.SendNext(new Vector3(-transform.position.x, -transform.position.y + y_offset, -transform.position.z)); //Solo se envía si se está moviendo.
            stream.SendNext(stunned);
            stream.SendNext(new Vector2(-GetComponent<Rigidbody2D>().velocity.x, -GetComponent<Rigidbody2D>().velocity.y));
            //stream.SendNext(photonView.ViewID);
        }
        else if (stream.IsReading)
        {
            //float playerLag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            //Vector3 lastSmoothPos = smoothMove;
            //Vector3 facingDir = (lastSmoothPos - transform.position).normalized;
            smoothMove = (Vector3)stream.ReceiveNext();
            stunned = (bool)stream.ReceiveNext();
            GetComponent<Rigidbody2D>().velocity = (Vector2)stream.ReceiveNext();
            //smoothMove = smoothMove + (facingDir * (speed * playerLag));
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

    private Vector3 putZAxis(Vector3 vec) { return new Vector3(vec.x, vec.y, goal.transform.position.z); }

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
        if (!mg.GameOn && fightDir == null) return;
        if (ball)
        {
            if (ball.GetComponent<Ball>().direction == Vector2.zero)
            {
                PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).GetComponent<Ball>().shootPosition = new Vector2(_dir[2], _dir[3]);
                PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).GetComponent<Ball>().direction = new Vector2(_dir[0], _dir[1]);
                if(info.Sender.IsMasterClient) PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).GetComponent<Ball>().shooterIsMaster = true;
                else PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).GetComponent<Ball>().shooterIsMaster = false;
                //ball.GetComponent<Ball>().shoot = true;
                PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).GetComponent<Ball>().shootTimeRef = Time.time;

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
    public void Lose(bool toGoal = false)
    {
        Debug.Log(gameObject.name + " from " + gameObject.transform.parent.name + " lost the Fight");
        stunned = true;
        if(toGoal)
        {
            float[] dir = { mg.myIA_Players[3].transform.position.x, mg.myIA_Players[3].transform.position.y, ball.transform.position.x, ball.transform.position.y };
            photonView.RPC("ShootBall", RpcTarget.AllViaServer, dir);
        }
        else if (ball)
        {
            ball.transform.parent = null;
            ball = null;
        }
        if(!mg.GameOn)mg.photonView.RPC("resumeGame", RpcTarget.AllViaServer);
        StartCoroutine(Blink(4.0f));
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
            if (!mg.GameOn) endTime += 0.4f;
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
        if (formationPos == IA_manager.formationPositions.GOALKEEPER && goalKeeperRef + 5.0f < Time.time) goalKeeperRef = Time.time;
    }

    [PunRPC]
    public void SetName(string name)
    {
        //transform.GetChild(0).GetComponentInChildren<Text>().text = "O";
        SpriteRenderer renderer = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        Color rival = new Color(1, 0, 0, 0.5f);
        Color local = new Color(0, 0, 1, 0.5f);
        if (photonView.IsMine) renderer.color = local;
        else renderer.color = rival;
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
        else if (formationPos != IA_manager.formationPositions.GOALKEEPER && ball)
        {
            objective[1] = goal.transform.position.y;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        else if (mg.fightRef + 2.0f >= Time.time && formationPos != IA_manager.formationPositions.GOALKEEPER && mg.HasTheBall() == 2 && Vector2.Distance(GameObject.FindGameObjectWithTag("Ball").transform.position, transform.position) < 2.4f)
        {
            Vector2 retreatPos = GameObject.FindGameObjectWithTag("Ball").transform.position + (transform.position - GameObject.FindGameObjectWithTag("Ball").transform.position).normalized * 2.5f;
            objective[0] = retreatPos.x;
            objective[1] = retreatPos.y;
        }

        objective[2] = transform.position.y / 100.0f;
        transform.position = new Vector3(transform.position.x, transform.position.y, objective[2]);

        playerObjective =  new Vector3(objective[0], objective[1], objective[2]);
    }

    private bool SameTeam(GameObject P1, GameObject P2)
    {
        if (P1.transform.parent == P2.transform.parent) return true;
        else return false;
    }

    public void SetStats()
    {
        stats = new Stats(characterBasic.info.atk, characterBasic.info.teq, characterBasic.info.def);
    }

    public void RepositionPlayer()
    {
        transform.gameObject.layer = 0;
        transform.position = startPosition;
        //Lose();
        stunned = true;
        StartCoroutine(Blink(2.0f));
        GameObject.Destroy(actualLine);
        onMove = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
        if (!mg.GameOn || stunned) return;
        float detectionDist;
        detectionDist = GetComponent<CircleCollider2D>().radius = 0.75f;
        GameObject[] rivals;
        bool foundCovered = false;

        rivals = mg.myIA_Players;
        foreach (GameObject rival in rivals)
        {
            if (Vector2.Distance(rival.transform.position, transform.position) < detectionDist && !rival.GetComponent<MyPlayer>().stunned)
            {
                if (ball != null && rival.GetComponent<MyPlayer>().ball == null && mg.fightRef + 2.0f < Time.time)
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
                    mg.photonView.RPC("chooseDirection", RpcTarget.All, gameObject.GetComponent<MyPlayer>().photonView.ViewID, rival.GetComponent<MyPlayer>().photonView.ViewID);
                    //else photonView.RPC("chooseDirection", RpcTarget.AllViaServer, rival.GetComponent<MyPlayer>().photonView.ViewID, gameObject.GetComponent<MyPlayer>().photonView.ViewID);
                    return;
                    //mg.chooseDirection(gameObject.GetComponent<MyPlayer>(), other.GetComponent<MyPlayer>());
                }
                else if (ball == null) foundCovered = covered = true;
            }
            else if (!foundCovered) covered = false;
        }

        if (PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID)
            .transform.parent == null && Vector2.Distance(GameObject.FindGameObjectWithTag("Ball").transform.position, 
            transform.position - new Vector3(0, 0.5f, 0)) < detectionDist && !stunned && mg.GameStarted && 
            ((mg.lastPlayer != null && mg.lastPlayer != gameObject &&
            mg.lastPlayer.transform.parent.gameObject == transform.parent.gameObject) ||
            (PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID)
            .GetComponent<Ball>().shootTimeRef + 0.15f < Time.time && mg.lastPlayer != gameObject) || 
            PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID)
            .GetComponent<Ball>().shootTimeRef + 0.5f < Time.time))
        {
            photonView.RPC("GetBall", RpcTarget.All);
        }
        else if(PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).transform.parent != transform) ball = null;

    }

    void checkGoal()
    {
                //Goal
                int ia_Idx = 3;
                if (ball.transform.position.y / Mathf.Abs(ball.transform.position.y)
                    != mg.myIA_Players[ia_Idx].transform.position.y / Mathf.Abs(mg.myIA_Players[ia_Idx].transform.position.y)) return;
        
        mg.photonView.RPC("ChooseShoot", RpcTarget.All, gameObject.GetComponent<MyPlayer>().photonView.ViewID, mg.myIA_Players[ia_Idx].GetComponent<MyPlayer>().photonView.ViewID);
      
    }

    IEnumerator SetAnimatorValues()
    {
        yield return new WaitForSeconds(0.25f);

        try
        {
            if (photonView.IsMine)
            {
                if (mg.HasTheBall() == 2 || mg.HasTheBall() == 0) animDir = (GameObject.FindGameObjectWithTag("Ball").transform.position - transform.position).normalized;
                else animDir = (playerObjective - transform.position).normalized;
            }
            else
            {
                if (mg.HasTheBall() == 1 || mg.HasTheBall() == 0) animDir = (GameObject.FindGameObjectWithTag("Ball").transform.position - transform.position).normalized;
                else animDir = (smoothMove - transform.position).normalized;
            }

            if (formationPos == IA_manager.formationPositions.GOALKEEPER)
            {
                float inverseFactor;
                if (velocity0 >= 0.01) inverseFactor = -1.0f;
                else inverseFactor = 1.0f;
                animDir = new Vector2(animDir.x, Mathf.Abs(animDir.y) * (inverseFactor * (transform.position.y / Mathf.Abs(transform.position.y))));

            }

            if (animDir.y > 0 && ball != null) ball.transform.localPosition = new Vector3(ball.transform.localPosition.x, ball.transform.localPosition.y, 0.0005f);
            else if (ball != null) ball.transform.localPosition = new Vector3(ball.transform.localPosition.x, ball.transform.localPosition.y, -0.0005f);

            animator.SetFloat("DirectionX", animDir.x);
            animator.SetFloat("DirectionY", animDir.y);
            if (velocity0 < 0.01)
                animator.SetBool("Moving", false);
            else
                animator.SetBool("Moving", true);

            if (mg.GameOn != animator.enabled)
                animator.enabled = mg.GameOn;
            StartCoroutine(SetAnimatorValues());
        }
        catch(NullReferenceException e)
        {
            StartCoroutine(SetAnimatorValues());
        }
    }
}
