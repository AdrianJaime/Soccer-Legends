using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviourPun, IPunObservable
{
    enum fightState { FIGHT, SHOOT, NONE };

    public GameObject player1Prefab, player2Prefab, ballPrefab, directionSlide, specialSlide, scoreBoard, energyBar;
    [SerializeField]
    GameObject timmer;
    [SerializeField]
    GameObject energyNumbers;
    [SerializeField]
    GameObject statsUI;
    public bool GameStarted = false, GameOn = true;
    public GameObject[] _team;
    public GameObject[] myIA_Players;
    public float eneregyFill;
    public GameObject lastPlayer;
    [SerializeField]
    Animator animator;
    private List<int> touchesIdx;
    private int fingerIdx = -1;
    private float enemySpecialBar = 0;
    private int energySegments = 0;

    private float timeStart = 0;
    public float fightRef = 0;
    private int fightingPlayer = 0, fightingIA = 0;
    private string fightDir;
    private bool shooting = false;
    private Vector2 score = new Vector2(0, 0);
    private int randomValue;
    fightState state = fightState.FIGHT;

    Vector2[] swipes;

    [SerializeField]
    GameObject field;

    //Confrontation
    [SerializeField]
    Image myConfrontationImage;
    [SerializeField]
    Image iaConfrontationImage;
    [SerializeField]
    Image mySpecialAtqImage;
    [SerializeField]
    Image iaSpecialAtqImage;
    List<SpriteRenderer> confontationAnimSprites;

    [SerializeField]
    GameObject introObj;
    [SerializeField]
    Animator outroObj;
    [SerializeField]
    Text playerOutroPoints;
    [SerializeField]
    Text enemyOutroPoints;

    //Hardcoded bug fixes
    int frameCount = 0;
    int goalRefFrame;

    // Start is called before the first frame update
    void Start()
    {
        confontationAnimSprites = new List<SpriteRenderer>();
        touchesIdx = new List<int>();
        fingerIdx = -1;
        swipes = new Vector2[2];
        SpawnPlayers();
    }

    private void Update()
    {
        //Wait for player comprobation;
        if (!GameStarted) return;

        if (touchesIdx.Count == 1 && Input.touchCount == 0)
        {
            frameCount++;
            if (frameCount == 2)
            {
                touchesIdx.Clear();
                frameCount = 0;
            }
        }

        if(goalRefFrame + 60 == Time.frameCount) photonView.RPC("Goal", RpcTarget.AllViaServer);

        if (timeStart + 60 < Time.time || score.x == 3 || score.y == 3)
        {
            StartCoroutine(outro());
        }
        else
        {
            if (!GameOn) timeStart += Time.deltaTime;
            timmer.GetComponent<TextMeshProUGUI>().SetText(((int)(timeStart + 60 - Time.time)).ToString());
        }

        if (!GameOn && GameStarted)
        {
            if ((Input.touchCount == 1 && touchesIdx.Count == 0 || fingerIdx != -1))
            {
                Touch swipe;
                if (fingerIdx != 0) fingerIdx = getTouchIdx();
                try
                {
                    swipe = Input.GetTouch(fingerIdx);
                }catch (ArgumentException e)
                {
                    releaseTouchIdx(fingerIdx);
                    fingerIdx = -1;
                    return;
                }
                if (swipe.phase == TouchPhase.Began)
                {
                    swipes[0] = swipe.position;
                }
                else if (swipe.phase == TouchPhase.Ended && PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir == null)
                {
                    directionSlide.SetActive(false); specialSlide.SetActive(false);
                    swipes[1] = swipe.position;
                    if(state == fightState.FIGHT)
                    {
                        if (swipes[0].y > swipes[1].y && Vector2.Angle(new Vector2(0, -1), new Vector2(swipes[1].x - swipes[0].x, swipes[1].y - swipes[0].y)) <= 60.0f && energyBar.GetComponent<Slider>().value + energySegments > 1.0f)
                        {
                            PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Special";
                            float energy = energyBar.GetComponent<Slider>().value + energySegments;
                            energyBar.GetComponent<Slider>().value = energySegments = 0;
                            energy -= 1.0f;
                            while (energy > 1.0f) { energy -= 1.0f; energySegments++; }
                            energyBar.GetComponent<Slider>().value = energy;
                            photonView.RPC("specialUpgrade", RpcTarget.All, fightingPlayer);
                        }
                        else if (swipes[0].x > swipes[1].x)
                        {
                            PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Risky";
                            if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().ball != null)
                                photonView.RPC("statsUpdate", RpcTarget.All, fightingPlayer, 0, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.technique / 2, 0);
                            else photonView.RPC("statsUpdate", RpcTarget.All, fightingPlayer, 0, 0, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.defense / 2);
                        }
                        else PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Normal";
                    }
                    else if(state == fightState.SHOOT)
                    {
                        if (swipes[0].y > swipes[1].y && Vector2.Angle(new Vector2(0, -1), new Vector2(swipes[1].x - swipes[0].x, swipes[1].y - swipes[0].y)) <= 60.0f && energyBar.GetComponent<Slider>().value + energySegments > 1.0f)
                        {
                            PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Special";
                            float energy = energyBar.GetComponent<Slider>().value + energySegments;
                            energyBar.GetComponent<Slider>().value = energySegments = 0;
                            energy -= 1.0f;
                            while (energy > 1.0f) { energy -= 1.0f; energySegments++; }
                            energyBar.GetComponent<Slider>().value = energy;
                            photonView.RPC("specialUpgrade", RpcTarget.All, fightingPlayer);
                        }
                        else if (swipes[0].x > swipes[1].x)
                        {
                            PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Risky";
                            if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().formationPos == IA_manager.formationPositions.GOALKEEPER)
                                photonView.RPC("statsUpdate", RpcTarget.All, fightingPlayer, 0, 0, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.defense / 2);
                            else photonView.RPC("statsUpdate", RpcTarget.All, fightingPlayer, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.shoot / 2, 0, 0);
                        }
                        else PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Normal";
                    }

                    if (!PhotonNetwork.IsMasterClient) photonView.RPC("setFightDir", RpcTarget.MasterClient, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir);

                    Debug.Log(PhotonView.Find(fightingPlayer).name + " from " + PhotonView.Find(fightingPlayer).transform.parent.name +
                    " chose direction " + PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir);
                    Debug.Log(PhotonView.Find(fightingIA).name + " from " + PhotonView.Find(fightingIA).transform.parent.name +
                        " chose direction " + PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir);
                    releaseTouchIdx(fingerIdx);
                    fingerIdx = -1;
                }
            }
            updateUI_Stats();
            if (PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir != null &&
            PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir != null && PhotonNetwork.IsMasterClient) Fight(); 
        }
        
        else if (GameStarted && GameOn)
        {
            if (fingerIdx != -1)
            {
                releaseTouchIdx(fingerIdx);
                fingerIdx = -1;
            }

            //Energy Bar
            if (energyBar.GetComponent<Slider>().value != 1) energyBar.GetComponent<Slider>().value += (eneregyFill * Time.deltaTime);
            else if (energySegments < 5) { energySegments++; energyBar.GetComponent<Slider>().value = 0; }
            energyNumbers.GetComponent<Text>().text = energyNumbers.transform.GetChild(0).GetComponent<Text>().text = energySegments.ToString();
        }
    }


    void SpawnPlayers()
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            GameObject localPlayer = PhotonNetwork.Instantiate(player1Prefab.name, player1Prefab.transform.position - new Vector3(0, 5, 0), player1Prefab.transform.rotation);
            _team = new GameObject[4];
            PhotonNetwork.Instantiate(ballPrefab.name, new Vector3(0, 0, 0), ballPrefab.transform.rotation);
            for (int i = 0; i < 4; i++)
            {
                _team[i] = localPlayer.transform.GetChild(i).gameObject;
            }
        }
        else
        {
            GameObject localPlayer = PhotonNetwork.Instantiate(player2Prefab.name, player2Prefab.transform.position - new Vector3(0, 5, 0), player2Prefab.transform.rotation);
            _team = new GameObject[4];
            for (int i = 0; i < 4; i++)
            {
                _team[i] = localPlayer.transform.GetChild(i).gameObject;
            }
        }
        StartCoroutine(intro());
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Send position to the other player. Stream == Getter.
    {
        if (stream.IsWriting)
        {
            stream.SendNext(GameStarted); //Solo se envía si se está moviendo.
            //stream.SendNext(fig);
        }
        else if (stream.IsReading)
        {
            GameStarted = (bool)stream.ReceiveNext();
            //if(info.Sender.IsMasterClient)
            //{

            //}
        }
    }

    public void StartButton()
    {
        Debug.Log("Player Count-> " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            releaseTouchIdx(fingerIdx);
            fingerIdx = -1;
            photonView.RPC("StartGame", RpcTarget.AllViaServer);
        }
    }

    void StartGame() {
        timeStart = Time.time;
        GameStarted = true; GameOn = true;
        scoreBoard.SetActive(true); directionSlide.SetActive(false); specialSlide.SetActive(false); statsUI.SetActive(false);
        GameObject T2 = GameObject.Find("Team 2(Clone)");
        GameObject T1 = GameObject.Find("Team 1(Clone)");
        myIA_Players = new GameObject[T1.transform.childCount];
        if (_team[0].transform.parent.gameObject != T1)
        {
            for (int i = 0; i < T1.transform.childCount; i++)
            {
                myIA_Players[i] = T1.transform.GetChild(i).gameObject;
            }
        }
        else
        {
            for (int i = 0; i < T2.transform.childCount; i++)
            {
                myIA_Players[i] = T2.transform.GetChild(i).gameObject;
            }
        }
    }

    [PunRPC]
    public void resumeGame()
    {
        GameStarted = true; GameOn = true;
        scoreBoard.SetActive(true); directionSlide.SetActive(false); specialSlide.SetActive(false); statsUI.SetActive(false);
        if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir != null &&
            PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir == "Special")
            photonView.RPC("specialDowngrade", RpcTarget.AllViaServer, fightingPlayer);
        statsUI.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
        statsUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
        for (int i = 0; i < myIA_Players.Length; i++)
        {
            _team[i].GetComponent<MyPlayer>().fightDir = null;
            myIA_Players[i].GetComponent<MyPlayer>().fightDir = null;
        }
        animator.ResetTrigger("Confrontation");
        animator.ResetTrigger("Battle");
        animator.ResetTrigger("Elude");
        animator.ResetTrigger("Lose");
        animator.ResetTrigger("Win");
        animator.ResetTrigger("SpecialAttack");
        fightRef = Time.time;
    }

    [PunRPC]
    public void chooseDirection(int _player1, int _player2)
    {
        if (!GameOn || PhotonView.Find(_player1).GetComponent<MyPlayer>().stunned || PhotonView.Find(_player2).GetComponent<MyPlayer>().stunned) return;
        if (_team[0].GetComponent<MyPlayer>().photonView.Owner != PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).Owner)
        {
            int aux = _player1;
            _player1 = _player2;
            _player2 = aux;
        }
        MyPlayer player1 = PhotonView.Find(_player1).GetComponent<MyPlayer>();
           // myPlayers[_player1].GetComponent<MyPlayer>();
        MyPlayer IA_Player = PhotonView.Find(_player2).GetComponent<MyPlayer>();
        //myIA_Players[_player2].GetComponent<MyPlayer>();
        try
        {
            if (player1.formationPos == IA_manager.formationPositions.GOALKEEPER)
            {
                player1.ball = GameObject.FindGameObjectWithTag("Ball");
                player1.ball.transform.localPosition = new Vector3(0, -0.5f, 0);
                IA_Player.GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer, false);
            }
            else if (IA_Player.formationPos == IA_manager.formationPositions.GOALKEEPER)
            {
                IA_Player.ball = GameObject.FindGameObjectWithTag("Ball");
                IA_Player.ball.transform.localPosition = new Vector3(0, -0.5f, 0);
                player1.GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer, false);
            }
            else if (!directionSlide.activeSelf)
            {
                if (HasTheBall() == 1)
                {
                    float[] arr = { player1.transform.position.x, player1.transform.position.y, player1.transform.position.z };
                    IA_Player.GetComponent<MyPlayer>().photonView.RPC("MoveTo", RpcTarget.AllViaServer, arr);
                    player1.GetComponent<MyPlayer>().photonView.RPC("MoveTo", RpcTarget.AllViaServer, arr);
                    //IA_Player.GetComponent<MyPlayer>().mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, _player2, _player1);
                }
                GameOn = false;
                directionSlide.SetActive(true);
                if (energyBar.GetComponent<Slider>().value + energySegments > 1.0f) specialSlide.SetActive(true);
                state = fightState.FIGHT;
                //if(player1 == null) player1 = PhotonView.Find(_player1).gameObject.GetComponent<MyPlayer>();
                //if(IA_Player == null) IA_Player = PhotonView.Find(_player2).gameObject.GetComponent<MyPlayer>();
                player1.fightDir = null;
                IA_Player.fightDir = null;
                fightingPlayer = _player1;
                fightingIA = _player2;
                //GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                myConfrontationImage.sprite = player1.confrontationSprite;
                iaConfrontationImage.sprite = IA_Player.confrontationSprite;
                mySpecialAtqImage.sprite = player1.specialSprite;
                iaSpecialAtqImage.sprite = IA_Player.specialSprite;
                //Set stats UI
                statsUI.SetActive(true);
                GameObject playerWithBall, playerWithoutBall;
                if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().ball != null)
                {
                    playerWithBall = PhotonView.Find(fightingPlayer).gameObject;
                    playerWithoutBall = PhotonView.Find(fightingIA).gameObject;
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "TEQ "
                        + playerWithBall.GetComponent<MyPlayer>().stats.technique.ToString();
                    statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                    statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text =
                        playerWithoutBall.GetComponent<MyPlayer>().stats.defense.ToString() + " DEF";
                }
                else
                {
                    playerWithoutBall = PhotonView.Find(fightingPlayer).gameObject;
                    playerWithBall = PhotonView.Find(fightingIA).gameObject;
                    statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                    statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text =
                        playerWithBall.GetComponent<MyPlayer>().stats.technique.ToString() + " TEQ";
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "DEF "
                        + playerWithoutBall.GetComponent<MyPlayer>().stats.defense.ToString();
                }
                playerWithBall.GetComponent<MyPlayer>().photonView.RPC("GetBall", RpcTarget.AllViaServer);
                StartCoroutine(enableConfrontationAnim());
                animator.SetTrigger("Confrontation");
            }
        }
        catch (NullReferenceException e)
        {
            photonView.RPC("resumeGame", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    public void setFightDir(string _fightDir) { PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir = _fightDir; }

    [PunRPC]
    public void Goal()
    {
        bool isLocal;
        if (_team[0].GetComponent<MyPlayer>().photonView.Owner != PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).Owner)
            isLocal = false;
        else isLocal = true;
            if (isLocal) score[0]++;
        else score[1]++;

        resumeGame();
        goalRefFrame = 0;
        lastPlayer = null;
        UpdateScoreBoard();
        Reposition();
        //photonView.RPC("UpdateScoreBoard", RpcTarget.AllViaServer);
        //photonView.RPC("Reposition", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void ChooseShoot(int _player1, int _player2)
    {
        if (!GameOn) return;
        if (_team[0].GetComponent<MyPlayer>().photonView.Owner != PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).Owner)
        {
            int aux = _player1;
            _player1 = _player2;
            _player2 = aux;
        }
        MyPlayer player1 = PhotonView.Find(_player1).GetComponent<MyPlayer>();
        // myPlayers[_player1].GetComponent<MyPlayer>();
        MyPlayer IA_Player = PhotonView.Find(_player2).GetComponent<MyPlayer>();
        //myIA_Players[_player2].GetComponent<MyPlayer>();
        try
        {
            if (!directionSlide.activeSelf)
            {
                GameOn = false;
                directionSlide.SetActive(true);
                if (energyBar.GetComponent<Slider>().value + energySegments > 1.0f) specialSlide.SetActive(true);
                state = fightState.SHOOT;
                //if(player1 == null) player1 = PhotonView.Find(_player1).gameObject.GetComponent<MyPlayer>();
                //if(IA_Player == null) IA_Player = PhotonView.Find(_player2).gameObject.GetComponent<MyPlayer>();
                player1.fightDir = null;
                IA_Player.fightDir = null;
                fightingPlayer = _player1;
                fightingIA = _player2;
                GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            myConfrontationImage.sprite = player1.confrontationSprite;
            iaConfrontationImage.sprite = IA_Player.confrontationSprite;
            mySpecialAtqImage.sprite = player1.specialSprite;
            iaSpecialAtqImage.sprite = IA_Player.specialSprite;
            //Set stats UI
            statsUI.SetActive(true);
            GameObject playerWithBall, goalkeeper;
            if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().formationPos != IA_manager.formationPositions.GOALKEEPER)
            {
                playerWithBall = PhotonView.Find(fightingPlayer).gameObject;
                goalkeeper = PhotonView.Find(fightingIA).gameObject;
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "ATQ "
                        + playerWithBall.GetComponent<MyPlayer>().stats.shoot.ToString();
                statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text =
                    goalkeeper.GetComponent<MyPlayer>().stats.defense.ToString() + " DEF";
            }
            else
            {
                goalkeeper = PhotonView.Find(fightingPlayer).gameObject;
                playerWithBall = PhotonView.Find(fightingIA).gameObject;
                statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text =
                        playerWithBall.GetComponent<MyPlayer>().stats.shoot.ToString() + " ATQ";
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "DEF "
                    + goalkeeper.GetComponent<MyPlayer>().stats.defense.ToString();
            }
            playerWithBall.GetComponent<MyPlayer>().photonView.RPC("GetBall", RpcTarget.AllViaServer);
            StartCoroutine(enableConfrontationAnim());
            animator.SetTrigger("Confrontation");
        }
        catch (NullReferenceException e)
        {
            photonView.RPC("resumeGame", RpcTarget.AllViaServer);
        }
    }

    private void Fight()
    {
        string fightType, fightResult;
        switch (state)
        {
            case fightState.FIGHT:
                GameObject playerWithBall, playerWithoutBall;
                if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().ball != null)
                {
                    playerWithBall = PhotonView.Find(fightingPlayer).gameObject;
                    playerWithoutBall = PhotonView.Find(fightingIA).gameObject;
                }
                else
                {
                    playerWithoutBall = PhotonView.Find(fightingPlayer).gameObject;
                    playerWithBall = PhotonView.Find(fightingIA).gameObject;
                }
                if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir == "Special" ||
                    PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir == "Special") fightType = "SpecialAttack";
                else fightType = "Battle";
                    randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer>().stats.technique + playerWithoutBall.GetComponent<MyPlayer>().stats.defense + 1);
                    Debug.Log(playerWithBall.name + " from " + playerWithBall.transform.parent.name + "has a technique of " + playerWithBall.GetComponent<MyPlayer>().stats.technique.ToString() +
                    " and a range between 1 and " + playerWithBall.GetComponent<MyPlayer>().stats.technique.ToString());
                    Debug.Log(playerWithoutBall.name + " from " + playerWithoutBall.transform.parent.name + "has a deffense of " + playerWithoutBall.GetComponent<MyPlayer>().stats.defense.ToString() +
                    " and a range between " + (playerWithBall.GetComponent<MyPlayer>().stats.technique + 1).ToString() + " and " +
                    (playerWithBall.GetComponent<MyPlayer>().stats.technique +
                    playerWithoutBall.GetComponent<MyPlayer>().stats.defense).ToString());
                    Debug.Log("Random value-> " + randomValue.ToString());
                if (randomValue > playerWithBall.GetComponent<MyPlayer>().stats.technique)
                {
                    fightResult = playerWithoutBall == PhotonView.Find(fightingPlayer).gameObject ? "Win" : "Lose";
                }
                else
                {
                    fightResult = playerWithBall == PhotonView.Find(fightingPlayer).gameObject ? "Win" : "Lose";
                    if (fightType != "SpecialAttack")
                    {
                        fightResult = fightType = "Elude";
                    }
                }
                
                photonView.RPC("setAnims", RpcTarget.AllViaServer, fightType, fightResult, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir, PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir, randomValue);
                break;
            case fightState.SHOOT:

                if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir == "Special" ||
                    PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir == "Special")
                    fightType = "SpecialAttack";
                else fightType = "Battle";

                GameObject goalkeeper;
                    if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().formationPos != IA_manager.formationPositions.GOALKEEPER)
                    {
                        playerWithBall = PhotonView.Find(fightingPlayer).gameObject;
                        goalkeeper = PhotonView.Find(fightingIA).gameObject;
                    }
                    else
                    {
                        goalkeeper = PhotonView.Find(fightingPlayer).gameObject;
                        playerWithBall = PhotonView.Find(fightingIA).gameObject;
                    }
                    {
                        randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer>().stats.shoot + goalkeeper.GetComponent<MyPlayer>().stats.defense + 1);
                        Debug.Log(playerWithBall.name + " from " + playerWithBall.transform.parent.name + "has a shoot of " + playerWithBall.GetComponent<MyPlayer>().stats.shoot.ToString() +
                        " and a range between 1 and " + playerWithBall.GetComponent<MyPlayer>().stats.shoot.ToString());
                        Debug.Log(goalkeeper.name + " from " + goalkeeper.transform.parent.name + "has a deffense of " + goalkeeper.GetComponent<MyPlayer>().stats.defense.ToString() +
                        " and a range between " + (playerWithBall.GetComponent<MyPlayer>().stats.shoot + 1).ToString() + " and " +
                        (playerWithBall.GetComponent<MyPlayer>().stats.shoot +
                        goalkeeper.GetComponent<MyPlayer>().stats.defense).ToString());
                        Debug.Log("Random value-> " + randomValue.ToString());
                        if (randomValue <= playerWithBall.GetComponent<MyPlayer>().stats.shoot)
                        {
                        fightResult = playerWithBall == PhotonView.Find(fightingPlayer).gameObject ? "Win" : "Lose";
                    }
                        else
                        {
                        fightResult = playerWithBall == PhotonView.Find(fightingPlayer).gameObject ? "Lose" : "Win";
                    }
                    }
                photonView.RPC("setAnims", RpcTarget.AllViaServer, fightType, fightResult, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir, PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir, randomValue);
                break;
            case fightState.NONE:
                return;

        }
        state = fightState.NONE;

    }

    [PunRPC]
    void updateUI_Stats()
    {
        string[] uiStats = new string[2];
        int[] uiNumStat = new int[2];
        MyPlayer[] fightingPlayers = new MyPlayer[2];
        fightingPlayers[0] = PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>();
        fightingPlayers[1] = PhotonView.Find(fightingIA).GetComponent<MyPlayer>();
        uiStats[0] = statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text.Substring(0, 3);
        uiStats[1] = statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text
            .Substring(statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text.Length - 3, 3);
        for (int i = 0; i < uiStats.Length; i++)
        {
            switch (uiStats[i])
            {
                case "DEF":
                    uiNumStat[i] = fightingPlayers[i].stats.defense;
                    break;
                case "TEQ":
                    uiNumStat[i] = fightingPlayers[i].stats.technique;
                    break;
                case "ATQ":
                    uiNumStat[i] = fightingPlayers[i].stats.shoot;
                    break;
            }
        }

        //Set dimensions
        float xScale_0 = 0.65f;
        float xScale_1 = -0.65f;
        float diff;
        diff = (float)(uiNumStat[0] - uiNumStat[1]) / 50.0f;
        if (diff > 0.30f) diff = 0.30f;
        else if (diff < -0.30f) diff = -0.30f;
        xScale_0 += diff;
        xScale_1 += diff;
        statsUI.transform.GetChild(0).localScale = new Vector3(xScale_0, 1, 1);
        statsUI.transform.GetChild(1).localScale = new Vector3(xScale_1, 1, 1);

        //Set Values
        statsUI.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = uiNumStat[0];
        statsUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = uiNumStat[1];

        //Set text
        statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = uiStats[0] + " "
                        + uiNumStat[0].ToString();
        statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
        statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text =
            uiNumStat[1].ToString() + " " + uiStats[1];
    }

    [PunRPC]
    public void statsUpdate(int _id, int _atq, int _teq, int _def)
    {
        PhotonView.Find(_id).GetComponent<MyPlayer>().stats.shoot += _atq;
        PhotonView.Find(_id).GetComponent<MyPlayer>().stats.technique += _teq;
        PhotonView.Find(_id).GetComponent<MyPlayer>().stats.defense += _def;
    }

    [PunRPC]
    public void specialUpgrade(int _id)
    {
        //En el futuro mirar lo que hace el especial
        PhotonView.Find(_id).GetComponent<MyPlayer>().stats.shoot *= 3;
        PhotonView.Find(_id).GetComponent<MyPlayer>().stats.defense *= 3;
        PhotonView.Find(_id).GetComponent<MyPlayer>().stats.technique *= 3; 
    }

    [PunRPC]
    public void specialDowngrade(int _id)
    {
        //En el futuro mirar lo que hace el especial
        PhotonView.Find(_id).GetComponent<MyPlayer>().stats.shoot /= 3;
        PhotonView.Find(_id).GetComponent<MyPlayer>().stats.defense /= 3;
        PhotonView.Find(_id).GetComponent<MyPlayer>().stats.technique /= 3;
    }

    [PunRPC]
    public void setStrategyBonus(IA_manager.strategy lastStrat, int _id)
    {
        GameObject[] _team = new GameObject[PhotonView.Find(_id).transform.parent.childCount];
        for (int i = 0; i < _team.Length; i++) _team[i] = PhotonView.Find(_id).transform.parent.GetChild(i).gameObject;
        foreach (GameObject player in _team)
        {
            MyPlayer playerScript = player.GetComponent<MyPlayer>();
            switch (player.transform.parent.GetComponent<PVP_IA_manager>().teamStrategy)
            {
                case IA_manager.strategy.DEFFENSIVE:
                    if (lastStrat == IA_manager.strategy.OFFENSIVE)
                    {
                        playerScript.stats.shoot = playerScript.stats.shoot - playerScript.stats.shoot / 3;
                        playerScript.stats.technique = playerScript.stats.technique - playerScript.stats.technique / 5;
                    }
                    playerScript.stats.defense = playerScript.stats.defense + playerScript.stats.defense / 2;
                    playerScript.stats.technique = playerScript.stats.technique + playerScript.stats.technique / 4;
                    break;
                case IA_manager.strategy.EQUILIBRATED:
                    if (lastStrat == IA_manager.strategy.OFFENSIVE)
                    {
                        playerScript.stats.shoot = playerScript.stats.shoot - playerScript.stats.shoot / 3;
                        playerScript.stats.technique = playerScript.stats.technique - playerScript.stats.technique / 5;
                    }
                    else if (lastStrat == IA_manager.strategy.DEFFENSIVE)
                    {
                        playerScript.stats.defense = playerScript.stats.defense - playerScript.stats.defense / 3;
                        playerScript.stats.technique = playerScript.stats.technique - playerScript.stats.technique / 5;
                    }
                    break;
                case IA_manager.strategy.OFFENSIVE:
                    if (lastStrat == IA_manager.strategy.DEFFENSIVE)
                    {
                        playerScript.stats.defense = playerScript.stats.defense - playerScript.stats.defense / 3;
                        playerScript.stats.technique = playerScript.stats.technique - playerScript.stats.technique / 5;
                    }
                    playerScript.stats.shoot = playerScript.stats.shoot + playerScript.stats.shoot / 2;
                    playerScript.stats.technique = playerScript.stats.technique + playerScript.stats.technique / 4;
                    break;
            }
        }
    }

    [PunRPC]
    public void setAnims(string fightType, string fightResult, string fightDirLocal, string fightDirRival, int _randomValue)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            string aux = fightDirLocal;
            fightDirLocal = fightDirRival;
            fightDirRival = aux;
            randomValue = _randomValue;
        }
        //Set booleans
        animator.SetBool("PlayerHasBall", PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().ball != null ? true : false);
        animator.SetBool("PlayerSpecial", fightDirLocal == "Special");
        animator.SetBool("EnemySpecial", fightDirRival == "Special");

        //Slider Effect waitTime 
        float waitTime = 0.0f;
        if (fightType == "SpecialAttack")
        {
            if (animator.GetBool("PlayerSpecial")) waitTime += 1.0f;
            if (animator.GetBool("EnemySpecial")) waitTime += 1.0f;
            animator.SetTrigger(fightType);
        }

        StartCoroutine(sliderEffect(waitTime, fightType, fightResult));

    }

    IEnumerator sliderEffect(float waitTime, string fightType, string fightResult)
    {
        yield return new WaitForSeconds(waitTime + Time.deltaTime);

        Slider localS = statsUI.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>();
        Slider rivalS = statsUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>();

        float sumMaxVal = localS.maxValue + rivalS.maxValue;
        float currentVal = 0.0f;
        float sumValue = Time.deltaTime * 3 * sumMaxVal;

        while (currentVal <= sumMaxVal)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            currentVal += sumValue;
            localS.value = currentVal;
            rivalS.value = currentVal - localS.maxValue;
        }

        currentVal = sumMaxVal;

        while (currentVal > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            currentVal -= sumValue;
            localS.value = currentVal;
            rivalS.value = currentVal - localS.maxValue;
        }

        currentVal = 0;

        if (!animator.GetBool("PlayerHasBall")) randomValue = (int)sumMaxVal - randomValue;

        while (currentVal <= randomValue)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            currentVal += sumValue;
            localS.value = currentVal;
            rivalS.value = currentVal - localS.maxValue;
        }

        //Set Results 
        animator.SetTrigger(fightType);
        if (PhotonNetwork.IsMasterClient || fightType == "Elude")
        {
            animator.SetTrigger(fightResult);
        }
        else
        {
            animator.SetTrigger(fightResult == "Win" ? "Lose" : "Win");
        }
    }

    public void fightResult(string anim)
    {
        switch (anim)
        {
            case "PlayerWinBattle":
                if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().formationPos == IA_manager.formationPositions.GOALKEEPER ||
                    PhotonView.Find(fightingIA).GetComponent<MyPlayer>().formationPos == IA_manager.formationPositions.GOALKEEPER)
                {
                    GameObject playerWithBall, goalkeeper;
                    if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().formationPos != IA_manager.formationPositions.GOALKEEPER)
                    {
                        //Gana el que chuta a puerta
                        playerWithBall = PhotonView.Find(fightingPlayer).gameObject;
                        goalkeeper = PhotonView.Find(fightingIA).gameObject;
                        if (goalkeeper.GetComponent<MyPlayer>().fightDir == "Risky")
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, goalkeeper.GetComponent<MyPlayer>().photonView.ViewID, 0, 0, -goalkeeper.GetComponent<MyPlayer>().stats.defense + goalkeeper.GetComponent<MyPlayer>().stats.defense / 3);
                        if (playerWithBall.GetComponent<MyPlayer>().fightDir == "Risky")
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, playerWithBall.GetComponent<MyPlayer>().photonView.ViewID, -playerWithBall.GetComponent<MyPlayer>().stats.shoot / 3, 0, 0);
                        float[] dir = { -1.0f * (goalkeeper.transform.position.x / Mathf.Abs(goalkeeper.transform.position.x)), goalkeeper.GetComponent<MyPlayer>().rival_goal.transform.position.y, playerWithBall.GetComponent<MyPlayer>().ball.transform.position.x, playerWithBall.GetComponent<MyPlayer>().ball.transform.position.y };
                        if (!playerWithBall.GetComponent<MyPlayer>().photonView.Owner.IsMasterClient)
                        {
                            dir[0] *= -1; dir[1] *= -1; dir[2] *= -1; dir[3] *= -1;
                        }
                        goalRefFrame = Time.frameCount;
                        playerWithBall.GetComponent<MyPlayer>().photonView.RPC("ShootBall", RpcTarget.AllViaServer, dir);
                    }
                    else
                    {
                        //Pierde el que chuta a puerta 
                        goalkeeper = PhotonView.Find(fightingPlayer).gameObject;
                        playerWithBall = PhotonView.Find(fightingIA).gameObject;
                        if (goalkeeper.GetComponent<MyPlayer>().fightDir == "Risky")
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, goalkeeper.GetComponent<MyPlayer>().photonView.ViewID, -goalkeeper.GetComponent<MyPlayer>().stats.shoot / 3, 0, 0);
                        if (playerWithBall.GetComponent<MyPlayer>().fightDir == "Risky")
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, 0, 0, -playerWithBall.GetComponent<MyPlayer>().stats.defense + playerWithBall.GetComponent<MyPlayer>().stats.defense / 3);
                        playerWithBall.GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer, true);
                    }
                }
                else
                {
                    //El jugador gana la batalla al rival
                    if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir == "Risky")
                    {
                        if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().ball != null)
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingPlayer, 0, -PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.technique / 3, 0);
                        else photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingPlayer, 0, 0, -PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.defense / 3);
                    }
                    if (PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir == "Risky")
                    {
                        if (PhotonView.Find(fightingIA).GetComponent<MyPlayer>().ball != null)
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingIA, 0, -PhotonView.Find(fightingIA).GetComponent<MyPlayer>().stats.technique + PhotonView.Find(fightingIA).GetComponent<MyPlayer>().stats.technique / 3, 0);
                        else photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingIA, 0, 0, -PhotonView.Find(fightingIA).GetComponent<MyPlayer>().stats.defense + PhotonView.Find(fightingIA).GetComponent<MyPlayer>().stats.defense / 3);
                    }
                    PhotonView.Find(fightingIA).GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer, false);
                }
                break;
            case "EnemyWinConfrontation":
                if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().formationPos == IA_manager.formationPositions.GOALKEEPER ||
                    PhotonView.Find(fightingIA).GetComponent<MyPlayer>().formationPos == IA_manager.formationPositions.GOALKEEPER)
                {
                    GameObject playerWithBall, goalkeeper;
                    if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().formationPos != IA_manager.formationPositions.GOALKEEPER)
                    {
                        //Pierde el que chuta a puerta
                        playerWithBall = PhotonView.Find(fightingPlayer).gameObject;
                        goalkeeper = PhotonView.Find(fightingIA).gameObject;
                        if (goalkeeper.GetComponent<MyPlayer>().fightDir == "Risky")
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, goalkeeper.GetComponent<MyPlayer>().photonView.ViewID, -goalkeeper.GetComponent<MyPlayer>().stats.shoot / 3, 0, 0);
                        if (playerWithBall.GetComponent<MyPlayer>().fightDir == "Risky")
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, 0, 0, -playerWithBall.GetComponent<MyPlayer>().stats.defense + playerWithBall.GetComponent<MyPlayer>().stats.defense / 3);
                        playerWithBall.GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer, true);
                    }
                    else
                    {
                        //Gana el que chuta a puerta 
                        goalkeeper = PhotonView.Find(fightingPlayer).gameObject;
                        playerWithBall = PhotonView.Find(fightingIA).gameObject;
                        if (goalkeeper.GetComponent<MyPlayer>().fightDir == "Risky")
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, goalkeeper.GetComponent<MyPlayer>().photonView.ViewID, 0, 0, -goalkeeper.GetComponent<MyPlayer>().stats.defense + goalkeeper.GetComponent<MyPlayer>().stats.defense / 3);
                        if (playerWithBall.GetComponent<MyPlayer>().fightDir == "Risky")
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, playerWithBall.GetComponent<MyPlayer>().photonView.ViewID, -playerWithBall.GetComponent<MyPlayer>().stats.shoot / 3, 0, 0);
                        float[] dir = { -1.0f * (goalkeeper.transform.position.x / Mathf.Abs(goalkeeper.transform.position.x)), goalkeeper.GetComponent<MyPlayer>().rival_goal.transform.position.y, playerWithBall.GetComponent<MyPlayer>().ball.transform.position.x, playerWithBall.GetComponent<MyPlayer>().ball.transform.position.y };
                        if (!playerWithBall.GetComponent<MyPlayer>().photonView.Owner.IsMasterClient)
                        {
                            dir[0] *= -1; dir[1] *= -1; dir[2] *= -1; dir[3] *= -1;
                        }
                        goalRefFrame = Time.frameCount;
                        playerWithBall.GetComponent<MyPlayer>().photonView.RPC("ShootBall", RpcTarget.AllViaServer, dir);
                    }
                }
                else
                {
                    //El rival gana la batalla al jugador 
                    if (PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir == "Risky")
                    {
                        if (PhotonView.Find(fightingIA).GetComponent<MyPlayer>().ball != null)
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingIA, 0, -PhotonView.Find(fightingIA).GetComponent<MyPlayer>().stats.technique / 3, 0);
                        else photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingIA, 0, 0, -PhotonView.Find(fightingIA).GetComponent<MyPlayer>().stats.defense / 3);
                    }
                    if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir == "Risky")
                    {
                        if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().ball != null)
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingPlayer, 0, -PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.technique + PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.technique / 3, 0);
                        else photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingPlayer, 0, 0, -PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.defense + PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.defense / 3);
                    }
                    PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer, false);
                }
                break;
            case "PlayerDodge":
            case "EnemyDodge":
                if (animator.GetBool("PlayerHasBall"))
                {
                    //El jugador esquiva al rival
                    if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir == "Risky")
                        photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingPlayer, 0, -PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.technique / 3, 0);
                    if (PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir == "Risky")
                        photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingIA, 0, 0, -PhotonView.Find(fightingIA).GetComponent<MyPlayer>().stats.defense + PhotonView.Find(fightingIA).GetComponent<MyPlayer>().stats.defense / 3);
                    PhotonView.Find(fightingIA).GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer, false);
                }
                else
                {
                    //El rival esquiva al jugador 
                    if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir == "Risky")
                        photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingPlayer, 0, 0, -PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.defense + PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.defense / 3);
                    if (PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir == "Risky")
                        photonView.RPC("statsUpdate", RpcTarget.AllViaServer, fightingIA, 0, -PhotonView.Find(fightingIA).GetComponent<MyPlayer>().stats.technique / 3, 0);
                    PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer, false);
                }
                break;
        }
    }

    public int HasTheBall()
    {
        if (PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).transform.parent == null) return 0;
        for (int i = 0; i < _team.Length; i++)
        {
            MyPlayer player1 = _team[i].GetComponent<MyPlayer>();
            MyPlayer IA_Player = myIA_Players[i].GetComponent<MyPlayer>();
            if (player1.ball != null) return 1;
            else if (IA_Player.ball != null) return 2;
        }
        Debug.Log("late 0");
        return 0;
    }

    public GameObject FindWhoHasTheBall()
    {
        for (int i = 0; i < _team.Length; i++)
        {
            MyPlayer player1 = _team[i].GetComponent<MyPlayer>();
            MyPlayer IA_Player = myIA_Players[i].GetComponent<MyPlayer>();
            if (player1.ball != null) return player1.gameObject;
            else if (IA_Player.ball != null) return IA_Player.gameObject;
        }
        return null;
    }

    [PunRPC]
    public void UpdateScoreBoard()
    {
        scoreBoard.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(score[0].ToString());
        scoreBoard.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(score[1].ToString());
        Debug.Log(score[0].ToString() + score[1].ToString());
    }

    [PunRPC]
    public void Reposition()
    {
        GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().RepositionBall();
        for (int i = 0; i < _team.Length; i++)
        {
            _team[i].GetComponent<MyPlayer>().RepositionPlayer();
            myIA_Players[i].GetComponent<MyPlayer>().RepositionPlayer();
        }
        //GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.RPC("RepositionBall", RpcTarget.AllViaServer);
    }

    IEnumerator enableConfrontationAnim()
    {
        while (confontationAnimSprites.Count > 0) { yield return new WaitForSeconds(Time.deltaTime); }
        confontationAnimSprites.AddRange(field.GetComponentsInChildren<SpriteRenderer>(true));
        for (int i = 0; i < _team.Length; i++)
        {
            if (_team[i] != PhotonView.Find(fightingPlayer).gameObject) confontationAnimSprites.AddRange(_team[i].GetComponentsInChildren<SpriteRenderer>(true));
            if (myIA_Players[i] != PhotonView.Find(fightingIA).gameObject) confontationAnimSprites.AddRange(myIA_Players[i].GetComponentsInChildren<SpriteRenderer>(true));
        }

        while (!GameOn && confontationAnimSprites[0].color.r > 0.2f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            foreach (SpriteRenderer rend in confontationAnimSprites)
            {
                Color c = rend.color;
                c.r -= 0.085f;
                c.g -= 0.085f;
                c.b -= 0.085f;
                rend.color = c;
            }
        }

        while (!GameOn) { yield return new WaitForSeconds(Time.deltaTime); }

        while (confontationAnimSprites[0].color.r < 1.0f)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            foreach (SpriteRenderer rend in confontationAnimSprites)
            {
                Color c = rend.color;
                c.r += 0.085f;
                c.g += 0.085f;
                c.b += 0.085f;
                rend.color = c;
            }
        }
        confontationAnimSprites.Clear();
    }

    IEnumerator intro()
    {
        while (GameObject.Find("Team 1(Clone)") == null || GameObject.Find("Team 2(Clone)") == null)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        introObj.SetActive(true);

        yield return new WaitForSeconds(4.0f);
        StartGame();
        yield return new WaitForSeconds(2.0f);
        Destroy(introObj);
    }

    IEnumerator outro()
    {
        GameStarted = false;
        GameOn = false;
        outroObj.gameObject.SetActive(true);
        outroObj.SetTrigger("CallOutro");
        outroObj.SetBool("WIN", score[0] > score[1] ? true : false);
        playerOutroPoints.transform.GetChild(0).GetComponent<Text>().text = playerOutroPoints.text = score[0].ToString();
        enemyOutroPoints.transform.GetChild(0).GetComponent<Text>().text = enemyOutroPoints.text = score[1].ToString();

        yield return new WaitForSeconds(4.0f);
        SceneManager.LoadScene("MainMenuScene");
    }

    public int getTouchIdx()
    {
        if (touchesIdx.Count == 0)
        {
            touchesIdx.Add(0);
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                if (!touchesIdx.Contains(i))
                {
                    touchesIdx.Add(i);
                    return i;
                }
            }
        }
        return 0;
    }

    public void releaseTouchIdx(int idx)
    {
        if (touchesIdx.Contains(idx)) touchesIdx.Remove(idx);
        else return;
        //Camera
        if (GameObject.FindGameObjectWithTag("Ball").transform.GetChild(0).GetComponent<PVP_cameraMovement>().fingerIdx > idx)
            GameObject.FindGameObjectWithTag("Ball").transform.GetChild(0).GetComponent<PVP_cameraMovement>().fingerIdx--;
        //Manager
        if (fingerIdx > idx) fingerIdx--;

        //Players
        if (_team[0].GetComponent<MyPlayer>().fingerIdx > idx) _team[0].GetComponent<MyPlayer>().fingerIdx--;
        if (_team[1].GetComponent<MyPlayer>().fingerIdx > idx) _team[1].GetComponent<MyPlayer>().fingerIdx--;
        if (_team[2].GetComponent<MyPlayer>().fingerIdx > idx) _team[2].GetComponent<MyPlayer>().fingerIdx--;
    }

    public int getTotalTouches() { return touchesIdx.Count; }
}
