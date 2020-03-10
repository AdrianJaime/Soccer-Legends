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
    public GameObject ball, line;
    public bool onMove = false, stunned = false, colliding = false, covered = false;
    public string fightDir;
    public Vector3 playerObjective = Vector3.zero;
    public Vector2 startPosition = Vector2.zero;

    public IA_manager.formationPositions formationPos;

    private Vector3 smoothMove, aux;
    private GameObject actualLine;
    private PVE_Manager mg;
    private List<Vector3> points;
    public Collider2D goal, rival_goal;
    private float touchTime;
    private int shootFramesRef;
    private int passFrames = 0;
    public int fingerIdx = -1;

    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer characterSprite;
    public Sprite confrontationSprite;
    public Sprite specialSprite;
    strategyUI strategyScript;

    float goalKeeperRef;

    float velocity0 = 0;

    //IA
    bool iaPlayer;

    private void Start()
    {
        mg = GameObject.Find("Manager").GetComponent<PVE_Manager>();
        strategyScript = GameObject.Find("CallStrategiesButton").GetComponent<strategyUI>();
        if (transform.parent.name.Substring(0, 7) == "Team IA") iaPlayer = true;
        else iaPlayer = false;
        setPlayer();
        if(iaPlayer)stats = new Stats(5, 3,3);
        else stats = new Stats(7, 5, 5);
        int[] starr = { Random.Range(1, 10), Random.Range(1, 10), Random.Range(1, 10) };
        fightDir = null;
        //if (photonView.IsMine)
        //{
            
        if (iaPlayer)
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
        animator.SetBool("Moving", true);
        StartCoroutine(SetAnimatorValues());
    }

    void setPlayer()
    {
        List<CharacterBasic> teamInfo = iaPlayer ? StaticInfo.rivalTeam : StaticInfo.teamSelectedToPlay;
        switch (gameObject.transform.GetSiblingIndex())
        {
            case 0:
                formationPos = IA_manager.formationPositions.CIERRE;
                gameObject.name = "Cierre";
                stats = new Stats(teamInfo[2].info.atk, teamInfo[1].info.teq,
                    teamInfo[2].info.def);
                confrontationSprite = teamInfo[2].basicInfo.artworkConforntation;
                specialSprite = teamInfo[2].basicInfo.artworkSpecialAttack;
                animator.runtimeAnimatorController = teamInfo[2].basicInfo.animator_character;
                break;
            case 1:
                formationPos = IA_manager.formationPositions.ALA;
                gameObject.name = "Ala";
                stats = new Stats(teamInfo[0].info.atk, teamInfo[3].info.teq,
                    teamInfo[0].info.def);
                confrontationSprite = teamInfo[0].basicInfo.artworkConforntation;
                specialSprite = teamInfo[0].basicInfo.artworkSpecialAttack;
                animator.runtimeAnimatorController = teamInfo[0].basicInfo.animator_character;
                break;
            case 2:
                formationPos = IA_manager.formationPositions.PIVOT;
                gameObject.name = "Pivot";
                stats = new Stats(teamInfo[1].info.atk, teamInfo[2].info.teq,
                    teamInfo[1].info.def);
                confrontationSprite = teamInfo[1].basicInfo.artworkConforntation;
                specialSprite = teamInfo[1].basicInfo.artworkSpecialAttack;
                animator.runtimeAnimatorController = teamInfo[1].basicInfo.animator_character;

                break;
            case 3:
                formationPos = IA_manager.formationPositions.GOALKEEPER;
                speed *= 3;
                stats = new Stats(teamInfo[3].info.atk, teamInfo[0].info.teq,
                    teamInfo[3].info.def);
                confrontationSprite = teamInfo[3].basicInfo.artworkConforntation;
                specialSprite = teamInfo[3].basicInfo.artworkSpecialAttack;
                animator.runtimeAnimatorController = teamInfo[3].basicInfo.animator_character;
                break;
            default:

                break;
        }
        SetName(gameObject.name);
    }


    private void Update()
    {
        //if (photonView.IsMine) //Check if we're the local player
        //{
        if (mg.GameOn && !stunned)
        {
            if (formationPos == IA_manager.formationPositions.GOALKEEPER)
            {
                MoveTo(new float[] { GameObject.FindGameObjectWithTag("Ball").transform.position.x, transform.position.y, 0.0f });
                if (!iaPlayer && ball && goalKeeperRef + 5.0f < Time.time)
                {
                    Vector2 randShoot = new Vector2(Random.Range(-10.0f, 10.0f), Random.Range(1.0f, 5.0f));
                    ShootBall(new float[] { randShoot.x, randShoot.y, transform.position.x, transform.position.y });
                }
            }
            if (startPosition == Vector2.zero) startPosition = transform.position;
            if (!iaPlayer && ball != null) ProcessInputs();
            if (points.Count > 1 && mg.GameOn && !stunned) FollowLine();
            else if (playerObjective != Vector3.zero)
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
            checkCollisionDetection();
            if (iaPlayer && ball && Time.frameCount % 120 == 0) stablishNewShootCheck();
        }
        else GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
        if ((Input.touchCount > mg.getTotalTouches() || fingerIdx != -1))
        {
            if (fingerIdx == -1) fingerIdx = mg.getTouchIdx();
            Touch swipe;
            if (Input.touchCount > fingerIdx)swipe = Input.GetTouch(fingerIdx);
            else
            {
                mg.releaseTouchIdx(fingerIdx);
                fingerIdx = -1;
                return;
            }
            aux = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0.0f)));
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
                        if (goal.bounds.Contains(aux) && ball.GetComponent<Ball>().inArea) checkGoal();
                        else
                        {
                            //Pass
                            float[] dir = { aux.x, aux.y, ball.transform.position.x, ball.transform.position.y };
                            ShootBall(dir);
                            //photonView.RPC("ShootBall", RpcTarget.AllViaServer, dir);
                        }
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

    public void ShootBall(float[] _dir)
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
            mg.lastPlayer = gameObject;
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
        if (fightDir == "Special" || fightDir == "Normal")
        {
            float[] dir;
            if (iaPlayer)dir = new float[] { mg.myPlayers[3].transform.position.x, mg.myPlayers[3].transform.position.y, ball.transform.position.x, ball.transform.position.y };
            else dir = new float[] { mg.myIA_Players[3].transform.position.x, mg.myIA_Players[3].transform.position.y, ball.transform.position.x, ball.transform.position.y };
            ShootBall(dir);
        }
        if (ball)
        {
            ball.transform.parent = null;
            ball = null;
        }
        mg.resumeGame();
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

    public void GetBall()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        ball.transform.parent = transform;
        ball.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.gameObject.layer = 2;
        if (iaPlayer) stablishNewShootCheck();
        else if (formationPos == IA_manager.formationPositions.GOALKEEPER && goalKeeperRef + 5.0f < Time.time) goalKeeperRef = Time.time;
        //if (!iaPlayer && (GameObject.FindGameObjectWithTag("MainCamera") != null))
        //{
        //    GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
        //    playerCamera.SetActive(true);
        //}
    }


    public void SetName(string name)
    {
        //transform.GetChild(0).GetComponentInChildren<Text>().text = "O";
        MeshRenderer meshRenderer = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
        // Get the current material applied on the GameObject
        Material oldMaterial = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;
        // Set the new material on the GameObject
        if(!iaPlayer)
        meshRenderer.material = mg.local;
        else meshRenderer.material = mg.rival;
    }

    public void IsColliding(bool isIt)
    {
        colliding = isIt;
        if(!isIt)playerObjective = Vector3.zero;
    }

    public void MoveTo(float[] objective)
    {
        if (formationPos == IA_manager.formationPositions.GOALKEEPER && Mathf.Abs(objective[0]) > 1.25f)
            objective[0] = (objective[0] / Mathf.Abs(objective[0])) * 1.25f;
        else if (formationPos != IA_manager.formationPositions.GOALKEEPER && ball)
            objective[1] = goal.transform.position.y;
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
        //Lose();
        stunned = true;
        StartCoroutine(Blink(2.0f));
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
        StartCoroutine(checkIA_Shoot_After_Time(Random.Range(1.0f, 2.0f)));
    }

    

    void checkCollisionDetection()
    {
        float detectionDist;
        detectionDist = GetComponent<CircleCollider2D>().radius = 0.75f;
        GameObject[] rivals;
        bool foundCovered = false;

        if (iaPlayer) rivals = mg.myPlayers;
        else rivals = mg.myIA_Players;
        foreach (GameObject rival in rivals)
        {
            if (Vector2.Distance(rival.transform.position, transform.position) < detectionDist && !rival.GetComponent<MyPlayer_PVE>().stunned)
            {
                if (ball != null && rival.GetComponent<MyPlayer_PVE>().ball == null && mg.fightRef + 2.0f < Time.time)
                {
                    int ia_Idx = 0;
                    int playerIdx = 0;
                    fightDir = null;
                    for (int i = 0; i < mg.myPlayers.Length; i++)
                    {
                        if (iaPlayer)
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
                    mg.chooseDirection(playerIdx, ia_Idx);
                    return;
                    //mg.chooseDirection(gameObject.GetComponent<MyPlayer>(), other.GetComponent<MyPlayer>());
                    //if (PhotonNetwork.IsMasterClient) mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, gameObject.GetComponent<MyPlayer>().photonView.ViewID, other.GetComponent<MyPlayer>().photonView.ViewID);
                    //else mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, other.GetComponent<MyPlayer>().photonView.ViewID, gameObject.GetComponent<MyPlayer>().photonView.ViewID);
                }
                else if (ball == null) foundCovered = covered = true;
            }
            else if (!foundCovered) covered = false;
        }

        if (GameObject.FindGameObjectWithTag("Ball").transform.parent == null && Vector2.Distance(GameObject.FindGameObjectWithTag("Ball").transform.position, transform.position - new Vector3(0, 0.5f, 0)) < detectionDist && !stunned && mg.GameStarted && shootFramesRef + 5 < Time.frameCount)
        {
            GetBall();
            //photonView.RPC("GetBall", RpcTarget.AllViaServer);
        }
        else if(GameObject.FindGameObjectWithTag("Ball").transform.parent != transform) ball = null;
        
    }

    void checkGoal()
    {
                //Goal
                int ia_Idx = 3;
                int playerIdx = 0;
                fightDir = null;

                if(ball.transform.position.y / Mathf.Abs(ball.transform.position.y)
                    != mg.myIA_Players[ia_Idx].transform.position.y / Mathf.Abs(mg.myIA_Players[ia_Idx].transform.position.y)) return;

                if (!iaPlayer) {
                    for (int i = 0; i < mg.myPlayers.Length; i++)
                    {

                        if (gameObject == mg.myPlayers[i])
                        {
                            playerIdx = i;
                            break;
                        }
                    }
                }
                mg.ChooseShoot(playerIdx, ia_Idx);
                //if(PhotonNetwork.IsMasterClient) mg.photonView.RPC("ChooseShoot", RpcTarget.AllViaServer, photonView.ViewID, findGoalKeeper().photonView.ViewID);
                //else mg.photonView.RPC("ChooseShoot", RpcTarget.AllViaServer, findGoalKeeper().photonView.ViewID, photonView.ViewID);
    }

    IEnumerator SetAnimatorValues()
    {
        yield return new WaitForSeconds(0.25f);
        Vector2 direction;

        if (!iaPlayer)
        {
            if (mg.HasTheBall() == 2 || mg.HasTheBall() == 0) direction = (GameObject.FindGameObjectWithTag("Ball").transform.position - transform.position).normalized;
            else direction = (playerObjective - transform.position).normalized;
        }
        else
        {
            if (mg.HasTheBall() == 1 || mg.HasTheBall() == 0) direction = (GameObject.FindGameObjectWithTag("Ball").transform.position - transform.position).normalized;
            else direction = (playerObjective - transform.position).normalized;
        }

        if (formationPos == IA_manager.formationPositions.GOALKEEPER)
        {
            float inverseFactor;
            if (velocity0 >= 0.01) inverseFactor =  -1.0f;
            else inverseFactor = 1.0f;
            direction = new Vector2(direction.x, Mathf.Abs(direction.y) * ( inverseFactor * (transform.position.y / Mathf.Abs(transform.position.y))));
            
        }

        if (direction.y > 0 && ball != null) ball.transform.localPosition = new Vector3(ball.transform.localPosition.x, ball.transform.localPosition.y, 0.05f);
        else if(ball != null) ball.transform.localPosition = new Vector3(ball.transform.localPosition.x, ball.transform.localPosition.y, -0.05f);

        animator.SetFloat("DirectionX", direction.x);
        animator.SetFloat("DirectionY", direction.y);
        if(velocity0<0.01)
            animator.SetBool("Moving", false);
        else
            animator.SetBool("Moving", true);

        if(mg.GameStarted && mg.GameOn!= animator.enabled)
            animator.enabled = mg.GameOn;
        StartCoroutine(SetAnimatorValues());
    }
}
