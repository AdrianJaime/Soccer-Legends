using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PVE_Manager : MonoBehaviour
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
    public GameObject[] myPlayers;
    public GameObject[] myIA_Players;
    public float eneregyFill;
    public GameObject lastPlayer;
    [SerializeField]
    Animator animator;
    private List<int> touchesIdx;
    private int fingerIdx = -1;
    private float enemySpecialBar = 0;
    private float energySegments = 0;

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
    Text mySpecialName;
    [SerializeField]
    Text iaSpecialName;

    [SerializeField]
    GameObject introObj;
    [SerializeField]
    Animator outroObj;
    [SerializeField]
    Text playerOutroPoints;
    [SerializeField]
    Text enemyOutroPoints;

    //Hardcoded bug fixes
    int goalRefFrame;
    int frameCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        confontationAnimSprites = new List<SpriteRenderer>();
        timeStart = Time.time;
        touchesIdx = new List<int>();
        fingerIdx = -1;
        swipes = new Vector2[2];
        SpawnPlayers();
    }

    private void Update()
    {
        if (touchesIdx.Count == 1 && Input.touchCount == 0)
        {
            frameCount++;
            if (frameCount == 2)
            {
                touchesIdx.Clear();
                frameCount = 0;
            }
        }

        if (goalRefFrame + 60 == Time.frameCount) {
            if (lastPlayer.transform.position.y > 0) Goal(true);
            else Goal(false);
        }

        //Match end/start manager
        if (GameStarted && timeStart + 60 < Time.time || score.x == 3 || score.y == 3) StartCoroutine(outro());
        else
        {
            if (!GameOn) timeStart += Time.deltaTime;
            timmer.GetComponent<TextMeshProUGUI>().SetText(((int)(timeStart + 60 - Time.time)).ToString());
        }
        if (!GameOn)
        {
           if(Input.touchCount == 1 && touchesIdx.Count == 0 || fingerIdx != -1) Fight();
            updateUI_Stats();
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
            if (enemySpecialBar != 1) enemySpecialBar += (eneregyFill * Time.deltaTime) / 5.0f;
        }
    }


    void SpawnPlayers()
    {
        GameObject localPlayer = Instantiate(player1Prefab.gameObject, player1Prefab.transform.position - new Vector3(0, 5, 0), player1Prefab.transform.rotation);
        myPlayers = new GameObject[4];
        Instantiate(ballPrefab.gameObject, new Vector3(0, localPlayer.transform.position.y, 0), ballPrefab.transform.rotation);
        for (int i = 0; i < 4; i++)
        {
            myPlayers[i] = localPlayer.transform.GetChild(i).gameObject;
        }

        //IA Rival
        GameObject IA_Rival = Instantiate(player2Prefab.gameObject, player2Prefab.transform.position + new Vector3(0, 5, 0), player2Prefab.transform.rotation);
        myIA_Players = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            IA_Rival.transform.GetChild(i).transform.position = localPlayer.transform.GetChild(i).transform.position * -1 + new Vector3(0, 1.0f, 0);
            myIA_Players[i] = IA_Rival.transform.GetChild(i).gameObject;
        }
        StartCoroutine(intro());
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Send position to the other player. Stream == Getter.
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(GameStarted); //Solo se envía si se está moviendo.
    //    }
    //    else if (stream.IsReading)
    //    {
    //        GameStarted = (bool)stream.ReceiveNext();
    //    }
    //}

    public void StartButton()
    {
        // if (PhotonNetwork.CurrentRoom.PlayerCount == 2) photonView.RPC("StartGame", RpcTarget.AllViaServer);
        
        releaseTouchIdx(fingerIdx);
        fingerIdx = -1;
    }

    public void StartGame() { GameStarted = true;  scoreBoard.SetActive(true); GameOn = true; }

    public void resumeGame()
    {
        GameStarted = true; GameOn = true;
        scoreBoard.SetActive(true); directionSlide.SetActive(false); specialSlide.SetActive(false); statsUI.SetActive(false);
        statsUI.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
        statsUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>(). value = 0;
        for (int i = 0; i < myIA_Players.Length; i++)
        {
            myPlayers[i].GetComponent<MyPlayer_PVE>().fightDir = null;
            myIA_Players[i].GetComponent<MyPlayer_PVE>().fightDir = null;
        }
        animator.ResetTrigger("Confrontation");
        animator.ResetTrigger("Battle");
        animator.ResetTrigger("Elude");
        animator.ResetTrigger("Lose");
        animator.ResetTrigger("Win");
        animator.ResetTrigger("SpecialAttack");
        fightRef = Time.time;
    }

    public void chooseDirection(int _player1, int _player2)
    {
        MyPlayer_PVE player1 = myPlayers[_player1].GetComponent<MyPlayer_PVE>();
        MyPlayer_PVE IA_Player = myIA_Players[_player2].GetComponent<MyPlayer_PVE>();

        if (!GameOn || player1.stunned || IA_Player.stunned) return;
        try
        {
            if (player1.formationPos == IA_manager.formationPositions.GOALKEEPER)
            {
                player1.GetComponent<MyPlayer_PVE>().GetBall();
                IA_Player.GetComponent<MyPlayer_PVE>().Lose();
            }
            else if (IA_Player.formationPos == IA_manager.formationPositions.GOALKEEPER)
            {
                IA_Player.GetComponent<MyPlayer_PVE>().GetBall();
                player1.GetComponent<MyPlayer_PVE>().Lose();
            }
            else if (!directionSlide.activeSelf)
            {
                GameOn = false;
                directionSlide.SetActive(true);
                if (player1.characterBasic.basicInfo.specialAttackInfo.specialAtack
                    .canUseSpecial(this, player1.gameObject, energyBar.GetComponent<Slider>().value + energySegments))
                    specialSlide.SetActive(true);
                state = fightState.FIGHT;
                player1.fightDir = null;
                IA_Player.fightDir = null;
                fightingPlayer = _player1;
                fightingIA = _player2;
                myConfrontationImage.sprite = player1.confrontationSprite;
                iaConfrontationImage.sprite = IA_Player.confrontationSprite;
                mySpecialAtqImage.sprite = player1.specialSprite;
                iaSpecialAtqImage.sprite = IA_Player.specialSprite;
                //Set stats UI
                statsUI.SetActive(true);
                GameObject playerWithBall, playerWithoutBall;
                if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().ball != null)
                {
                    playerWithBall = myPlayers[fightingPlayer];
                    playerWithoutBall = myIA_Players[fightingIA];
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "TEQ "
                        + playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique.ToString();
                    statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                    statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text =
                        playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense.ToString() + " DEF";
                }
                else
                {
                    playerWithoutBall = myPlayers[fightingPlayer];
                    playerWithBall = myIA_Players[fightingIA];
                    statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                    statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text =
                        playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique.ToString() + " TEQ";
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "DEF "
                        + playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense.ToString();
                }
                playerWithBall.GetComponent<MyPlayer_PVE>().GetBall();
                StartCoroutine(enableConfrontationAnim());
                animator.SetTrigger("Confrontation");
            }
        } catch (NullReferenceException e)
        {
            resumeGame();
        }
    }

    public void Goal(bool isLocal)
    {
        if (isLocal) score[0]++;
        else score[1]++;

        goalRefFrame = 0;
        lastPlayer = null;
        resumeGame();
        UpdateScoreBoard();
        Reposition();
        //photonView.RPC("UpdateScoreBoard", RpcTarget.AllViaServer);
        //photonView.RPC("Reposition", RpcTarget.AllViaServer);
    }


    public void ChooseShoot(int _player1, int _player2)
    {
        MyPlayer_PVE player1 = myPlayers[_player1].GetComponent<MyPlayer_PVE>();
        MyPlayer_PVE IA_Player = myIA_Players[_player2].GetComponent<MyPlayer_PVE>();
        try
        {
            if (!directionSlide.activeSelf)
            {
                GameOn = false;
                directionSlide.SetActive(true);
                if (player1.characterBasic.basicInfo.specialAttackInfo.specialAtack
                    .canUseSpecial(this, player1.gameObject, energyBar.GetComponent<Slider>().value + energySegments))
                    specialSlide.SetActive(true);
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
            if (fightingPlayer != 3)
            {
                playerWithBall = myPlayers[fightingPlayer];
                goalkeeper = myIA_Players[fightingIA];
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "ATQ "
                        + playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot.ToString();
                statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text =
                    goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense.ToString() + " DEF";
            }
            else
            {
                goalkeeper = myPlayers[fightingPlayer];
                playerWithBall = myIA_Players[fightingIA];
                statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text =
                        playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot.ToString() + " ATQ";
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "DEF "
                    + goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense.ToString();
            }
            playerWithBall.GetComponent<MyPlayer_PVE>().GetBall();
            StartCoroutine(enableConfrontationAnim());
            animator.SetTrigger("Confrontation");
        }
        catch (NullReferenceException e)
        {
            resumeGame();
        }
    }

    private void Fight()
    {
        string fightType, fightResult;
        switch (state)
        {
            case fightState.FIGHT:
                if (UnityEngine.Random.Range(0, 4) > 2 && myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo.specialAtack
                    .canUseSpecial(this, myIA_Players[fightingIA], enemySpecialBar * 5.0f))
                    myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = "Special";
                else myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = UnityEngine.Random.Range(0, 4) == 0 ? "Risky" : "Normal";
                if (fingerIdx != 0) fingerIdx = getTouchIdx();
                Touch swipe = Input.GetTouch(fingerIdx);
                if (swipe.phase == TouchPhase.Began)
                {
                    swipes[0] = swipe.position;
                }
                else if (swipe.phase == TouchPhase.Ended)
                {
                    swipes[1] = swipe.position;
                    if (swipes[0].y > swipes[1].y && Vector2.Angle(new Vector2(0, -1), new Vector2(swipes[1].x - swipes[0].x, swipes[1].y - swipes[0].y)) <= 60.0f && specialSlide.activeSelf)
                    {
                        float energy = energyBar.GetComponent<Slider>().value + energySegments;
                        energyBar.GetComponent<Slider>().value = energySegments = 0;
                        energy -= 1.0f;
                        while (energy > 1.0f) { energy -= 1.0f; energySegments++; }
                        energyBar.GetComponent<Slider>().value = energy;
                        myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Special";
                        specialUpgrade();
                    }
                    else if (swipes[0].x > swipes[1].x)
                    {
                        myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Risky";
                        if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().ball != null)
                            statsUpdate(false, 0, myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.technique / 2, 0);
                        else statsUpdate(false, 0, 0, myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.defense / 2);
                    }
                    else myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Normal";
                    if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir == "Special")
                    {
                        enemySpecialBar -= 1 / 5.0f;
                        specialUpgrade(true);
                    }
                    else if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                    {
                        if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().ball != null)
                            statsUpdate(true, 0, myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.technique / 2, 0);
                        else statsUpdate(true, 0, 0, myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.defense / 2);
                    }

                    directionSlide.SetActive(false); specialSlide.SetActive(false);

                    Debug.Log(myPlayers[fightingPlayer].name + " from " + myPlayers[fightingPlayer].transform.parent.name +
                        " chose direction " + myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir);
                    Debug.Log(myIA_Players[fightingIA].name + " from " + myIA_Players[fightingIA].transform.parent.name +
                        " chose direction " + myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir);

                    GameObject playerWithBall, playerWithoutBall;
                    if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().ball != null)
                    {
                        playerWithBall = myPlayers[fightingPlayer];
                        playerWithoutBall = myIA_Players[fightingIA];
                    }
                    else
                    {
                        playerWithoutBall = myPlayers[fightingPlayer];
                        playerWithBall = myIA_Players[fightingIA];
                    }

                    if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir == "Special" ||
                        myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir == "Special") fightType = "SpecialAttack";
                    else fightType = "Battle";
                    randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique + playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense + 1);
                    Debug.Log(playerWithBall.name + " from " + playerWithBall.transform.parent.name + "has a technique of " + playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique.ToString() +
                    " and a range between 1 and " + playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique.ToString());
                    Debug.Log(playerWithoutBall.name + " from " + playerWithoutBall.transform.parent.name + "has a deffense of " + playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense.ToString() +
                    " and a range between " + (playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique + 1).ToString() + " and " +
                    (playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique +
                    playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense).ToString());
                    Debug.Log("Random value-> " + randomValue.ToString());
                    if (randomValue > playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique)
                    {
                        fightResult = playerWithoutBall == myPlayers[fightingPlayer] ? "Win" : "Lose";
                    }
                    else
                    {
                        fightResult = playerWithBall == myPlayers[fightingPlayer] ? "Win" : "Lose";
                        if (fightType != "SpecialAttack")
                        {
                            fightResult = fightType = "Elude";
                        }
                    }
                    setAnims(fightType, fightResult);
                    releaseTouchIdx(fingerIdx);
                    fingerIdx = -1;
                }
                break;
            case fightState.SHOOT:
                if (UnityEngine.Random.Range(0, 4) > 0 && myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo.specialAtack
                    .canUseSpecial(this, myIA_Players[fightingIA], enemySpecialBar * 5.0f))
                    myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = "Special";
                else myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = UnityEngine.Random.Range(0, 3) == 0 ? "Risky" : "Normal";
                if (fingerIdx != 0) fingerIdx = getTouchIdx();
                swipe = Input.GetTouch(fingerIdx);
                if (swipe.phase == TouchPhase.Began)
                {
                    swipes[0] = swipe.position;
                }
                else if (swipe.phase == TouchPhase.Ended)
                {
                    swipes[1] = swipe.position;
                    if (swipes[0].y > swipes[1].y && Vector2.Angle(new Vector2(0, -1), new Vector2(swipes[1].x - swipes[0].x, swipes[1].y - swipes[0].y)) <= 60.0f && specialSlide.activeSelf)
                    {
                        float energy = energyBar.GetComponent<Slider>().value + energySegments;
                        energyBar.GetComponent<Slider>().value = energySegments = 0;
                        energy -= 1.0f;
                        while (energy > 1.0f) { energy -= 1.0f; energySegments++; }
                        energyBar.GetComponent<Slider>().value = energy;
                        myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Special";
                        specialUpgrade();
                    }
                    else if (swipes[0].x > swipes[1].x)
                    {
                        myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Risky";
                        if(fightingPlayer == 3)statsUpdate(false, 0, 0, myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.defense / 2);
                        else statsUpdate(false, myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.shoot / 2, 0, 0);
                    }
                    else myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Normal";
                    if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir == "Special"){
                        enemySpecialBar -= 1 / 5.0f;
                        specialUpgrade(true);
                    }
                    else if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                    {
                        if (fightingIA == 3) statsUpdate(true, 0, 0, myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.defense / 2);
                        else statsUpdate(true, myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.shoot / 2, 0, 0);
                    }

                    directionSlide.SetActive(false); specialSlide.SetActive(false);
                    Debug.Log(myPlayers[fightingPlayer].name + " from " + myPlayers[fightingPlayer].transform.parent.name +
                        " chose shooting " + myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir);
                    Debug.Log(myIA_Players[fightingIA].name + " from " + myIA_Players[fightingIA].transform.parent.name +
                        " chose " + myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir);

                    if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir == "Special" ||
                        myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir == "Special") fightType = "SpecialAttack";
                    else fightType = "Battle";

                    GameObject playerWithBall, goalkeeper;
                    if (fightingPlayer != 3)
                    {
                        playerWithBall = myPlayers[fightingPlayer];
                        goalkeeper = myIA_Players[fightingIA];
                    }
                    else
                    {
                        goalkeeper = myPlayers[fightingPlayer];
                        playerWithBall = myIA_Players[fightingIA];
                    }
                        randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot + goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense + 1);
                        Debug.Log(playerWithBall.name + " from " + playerWithBall.transform.parent.name + "has a shoot of " + playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot.ToString() +
                        " and a range between 1 and " + playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot.ToString());
                        Debug.Log(goalkeeper.name + " from " + goalkeeper.transform.parent.name + "has a deffense of " + goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense.ToString() +
                        " and a range between " + (playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot + 1).ToString() + " and " +
                        (playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot +
                        goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense).ToString());
                        Debug.Log("Random value-> " + randomValue.ToString());
                        if (randomValue <= playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot)
                        {
                            fightResult = playerWithBall == myPlayers[fightingPlayer] ? "Win" : "Lose";
                        }
                        else
                        {
                            fightResult = playerWithBall == myPlayers[fightingPlayer] ? "Lose" : "Win";
                        }
                    setAnims(fightType, fightResult);
                    releaseTouchIdx(fingerIdx);
                    fingerIdx = -1;
                    state = fightState.NONE;
                }
                break;
            case fightState.NONE:
                break;

        }
    }

    void updateUI_Stats()
    {
        string[] uiStats = new string[2];
        int[] uiNumStat = new int [2];
        MyPlayer_PVE[] fightingPlayers = new MyPlayer_PVE[2];
        fightingPlayers[0] = myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>();
        fightingPlayers[1] = myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>();
        uiStats[0] = statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text.Substring(0, 3);
        uiStats[1] = statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text
            .Substring(statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text.Length - 3, 3);
        for(int i = 0; i < uiStats.Length; i++)
        {
            switch(uiStats[i])
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
        if (diff > 0.20f) diff = 0.20f;
        else if (diff < -0.32f) diff = -0.20f;
        xScale_0 += diff;
        xScale_1 += diff;
        statsUI.transform.GetChild(0).localScale = new Vector3(xScale_0, 1, 1);
        statsUI.transform.GetChild(1).localScale = new Vector3(xScale_1, 1, 1);
        statsUI.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().handleRect.localScale = new Vector3(1 - diff, 1, 1);
        statsUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().handleRect.localScale = new Vector3(-1 - diff, 1, 1);

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

    public void statsUpdate(bool _ia, int _atq, int _teq, int _def)
    {
        if (_ia)
        {
            myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.shoot += _atq;
            myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.technique += _teq;
            myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.defense += _def;
        }
        else
        {
            myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.shoot += _atq;
            myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.technique += _teq;
            myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.defense += _def;
        }
    }

    void specialUpgrade(bool _ia = false)
    {
        SpecialAttackInfo specialInfo;
        GameObject local, rival;
        if (_ia)
        {
            local = myIA_Players[fightingIA];
            rival = myPlayers[fightingPlayer];
            specialInfo = local.GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo;
            iaSpecialName.text = specialInfo.name;
        }
        else
        {
            local = myPlayers[fightingPlayer];
            rival = myIA_Players[fightingIA];
            specialInfo = local.GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo;
            mySpecialName.text = specialInfo.name;
        }
        StartCoroutine(specialInfo.specialAtack.callSpecial(this, local, rival));
    }

    public void setStrategyBonus(int _strat)
    {
        IA_manager.strategy lastStrat = myPlayers[0].transform.parent.GetComponent<IA_manager>().teamStrategy;
        myPlayers[0].transform.parent.GetComponent<IA_manager>().teamStrategy = (IA_manager.strategy)_strat;
        foreach (GameObject player in myPlayers)
        {
            MyPlayer_PVE playerScript = player.GetComponent<MyPlayer_PVE>();
            switch (player.transform.parent.GetComponent<IA_manager>().teamStrategy)
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

    void setAnims(string fightType, string fightResult)
    {
        //Set booleans
        animator.SetBool("PlayerHasBall", myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().ball != null ? true : false);
        animator.SetBool("PlayerSpecial", myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir == "Special");
        animator.SetBool("EnemySpecial", myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir == "Special");

        //Slider Effect waitTime
        float waitTime = 0.0f;
        if(fightType == "SpecialAttack")
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
        float currentVal = localS.maxValue;
        float sumValue = Time.deltaTime * 3 * sumMaxVal;

        while(currentVal < sumMaxVal)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            currentVal += sumValue;
            localS.value = currentVal;
            rivalS.value = currentVal - localS.maxValue;
            localS.handleRect.GetComponent<Image>().enabled = currentVal <= localS.maxValue;
            rivalS.handleRect.GetComponent<Image>().enabled = currentVal - localS.maxValue > rivalS.minValue;
        }

        currentVal = sumMaxVal;

        while (currentVal > localS.minValue)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            currentVal -= sumValue;
            localS.value = currentVal;
            rivalS.value = currentVal - localS.maxValue;
            localS.handleRect.GetComponent<Image>().enabled = currentVal <= localS.maxValue;
            rivalS.handleRect.GetComponent<Image>().enabled = currentVal - localS.maxValue > rivalS.minValue;
        }

        currentVal = localS.minValue;

        if (!animator.GetBool("PlayerHasBall")) randomValue = (int)sumMaxVal - randomValue;

        while (currentVal < randomValue)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            currentVal += sumValue;
            localS.value = currentVal;
            rivalS.value = currentVal - localS.maxValue;
            localS.handleRect.GetComponent<Image>().enabled = currentVal <= localS.maxValue;
            rivalS.handleRect.GetComponent<Image>().enabled = currentVal - localS.maxValue > rivalS.minValue;
        }

        currentVal = randomValue;
        localS.value = currentVal;
        rivalS.value = currentVal - localS.maxValue;
        localS.handleRect.GetComponent<Image>().enabled = currentVal <= localS.maxValue;
        rivalS.handleRect.GetComponent<Image>().enabled = currentVal - localS.maxValue > rivalS.minValue;

        //Set Results
        animator.SetTrigger(fightType);
        animator.SetTrigger(fightResult);

        //Slider ending
        Image losingPlayer = localS.handleRect.GetComponent<Image>().enabled == false ? 
            localS.transform.GetChild(0).GetComponent<Image>() : rivalS.transform.GetChild(0).GetComponent<Image>();
        Color c;
        while (losingPlayer.color.r > 0.3f)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            c = losingPlayer.color;
            c.r -= 0.085f;
            c.g -= 0.085f;
            c.b -= 0.085f;
            losingPlayer.color = c;
        }

        while (!GameOn) { yield return new WaitForSeconds(Time.deltaTime); }

        c = losingPlayer.color;
        c.r = 1;
        c.g = 1;
        c.b = 1;
        losingPlayer.color = c;

        localS.handleRect.GetComponent<Image>().enabled = false;
        rivalS.handleRect.GetComponent<Image>().enabled = false;
    }

    public void fightResult(string anim)
    {
        switch (anim)
        {
            case "PlayerWinBattle":
                if (fightingIA == 3 || fightingPlayer == 3)
                {
                    GameObject playerWithBall, goalkeeper;
                    if (fightingPlayer != 3)
                    {
                        //Gana el que chuta a puerta
                        playerWithBall = myPlayers[fightingPlayer];
                        goalkeeper = myIA_Players[fightingIA];
                        if (goalkeeper.GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                            statsUpdate(!goalkeeper.transform.parent.GetComponent<IA_manager>().playerTeam, 0, 0, -goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense + goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense / 3);
                        if (playerWithBall.GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                            statsUpdate(!playerWithBall.transform.parent.GetComponent<IA_manager>().playerTeam, -playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot / 3, 0, 0);
                        playerWithBall.GetComponent<MyPlayer_PVE>().ShootBall(new float[] { -1.0f * (goalkeeper.transform.position.x / Mathf.Abs(goalkeeper.transform.position.x)), goalkeeper.GetComponent<MyPlayer_PVE>().rival_goal.transform.position.y, playerWithBall.GetComponent<MyPlayer_PVE>().ball.transform.position.x, playerWithBall.GetComponent<MyPlayer_PVE>().ball.transform.position.y });
                        goalRefFrame = Time.frameCount;
                    }
                    else
                    {
                        //Pierde el que chuta a puerta
                        goalkeeper = myPlayers[fightingPlayer];
                        playerWithBall = myIA_Players[fightingIA];
                        if (goalkeeper.GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                            statsUpdate(!goalkeeper.transform.parent.GetComponent<IA_manager>().playerTeam, -goalkeeper.GetComponent<MyPlayer_PVE>().stats.shoot / 3, 0, 0);
                        if (playerWithBall.GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                            statsUpdate(!playerWithBall.transform.parent.GetComponent<IA_manager>().playerTeam, 0, 0, -playerWithBall.GetComponent<MyPlayer_PVE>().stats.defense + playerWithBall.GetComponent<MyPlayer_PVE>().stats.defense / 3);
                        playerWithBall.GetComponent<MyPlayer_PVE>().Lose(true);
                    }
                }
                else
                {
                    //El jugador gana la batalla a la IA
                    if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                    {
                        if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().ball != null)
                            statsUpdate(false, 0, -myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.technique / 3, 0);
                        else statsUpdate(false, 0, 0, -myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.defense / 3);
                    }
                    if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                    {
                        if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().ball != null)
                            statsUpdate(true, 0, -myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.technique + myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.technique / 3, 0);
                        else statsUpdate(true, 0, 0, -myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.defense + myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.defense / 3);
                    }
                    myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().Lose();
                }
                break;
            case "EnemyWinConfrontation":
                if (fightingIA == 3 || fightingPlayer == 3)
                {
                    GameObject playerWithBall, goalkeeper;
                    if (fightingPlayer != 3)
                    {
                        //Pierde el que chuta a puerta
                        playerWithBall = myPlayers[fightingPlayer];
                        goalkeeper = myIA_Players[fightingIA];
                        if (goalkeeper.GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                            statsUpdate(!goalkeeper.transform.parent.GetComponent<IA_manager>().playerTeam, -goalkeeper.GetComponent<MyPlayer_PVE>().stats.shoot / 3, 0, 0);
                        if (playerWithBall.GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                            statsUpdate(!playerWithBall.transform.parent.GetComponent<IA_manager>().playerTeam, 0, 0, -playerWithBall.GetComponent<MyPlayer_PVE>().stats.defense + playerWithBall.GetComponent<MyPlayer_PVE>().stats.defense / 3);
                        playerWithBall.GetComponent<MyPlayer_PVE>().Lose(true);
                    }
                    else
                    {
                        //Gana el que chuta a puerta
                        goalkeeper = myPlayers[fightingPlayer];
                        playerWithBall = myIA_Players[fightingIA];
                        if (goalkeeper.GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                            statsUpdate(!goalkeeper.transform.parent.GetComponent<IA_manager>().playerTeam, 0, 0, -goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense + goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense / 3);
                        if (playerWithBall.GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                            statsUpdate(!playerWithBall.transform.parent.GetComponent<IA_manager>().playerTeam, -playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot / 3, 0, 0);
                        playerWithBall.GetComponent<MyPlayer_PVE>().ShootBall(new float[] { -1.0f * (goalkeeper.transform.position.x / Mathf.Abs(goalkeeper.transform.position.x)), goalkeeper.GetComponent<MyPlayer_PVE>().rival_goal.transform.position.y, playerWithBall.GetComponent<MyPlayer_PVE>().ball.transform.position.x, playerWithBall.GetComponent<MyPlayer_PVE>().ball.transform.position.y });
                        goalRefFrame = Time.frameCount;
                    }
                }
                else
                {
                    //La IA gana la batalla al jugador
                    if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                    {
                        if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().ball != null)
                            statsUpdate(false, 0, -myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.technique + myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.technique / 3, 0);
                        else statsUpdate(false, 0, 0, -myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.defense + myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.defense / 3);
                    }
                    if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                    {
                        if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().ball != null)
                            statsUpdate(true, 0, - myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.technique / 3, 0);
                        else statsUpdate(true, 0, 0, -myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.defense / 3);
                    }
                    myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().Lose();
                }
                break;
            case "PlayerDodge":
            case "EnemyDodge":
                if (animator.GetBool("PlayerHasBall"))
                {
                    //El jugador esquiva a la IA
                    if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                        statsUpdate(false, 0, -myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.technique / 3, 0);
                    if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                        statsUpdate(true, 0, 0, -myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.defense + myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.defense / 3);
                    myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().Lose();
                }
                else
                {
                    //La IA esquiva al jugador
                    if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                        statsUpdate(false, 0, 0, -myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.defense + myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.defense / 3);
                    if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir == "Risky")
                        statsUpdate(true, 0, -myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.technique / 3, 0);
                    myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().Lose();
                }
                break;
        }
    }

    public int HasTheBall()
    {
        if (GameObject.FindGameObjectWithTag("Ball").transform.parent == null) return 0;
        for (int i = 0; i < myPlayers.Length; i++)
        {
            MyPlayer_PVE player1 = myPlayers[i].GetComponent<MyPlayer_PVE>();
            MyPlayer_PVE IA_Player = myIA_Players[i].GetComponent<MyPlayer_PVE>();
            if (player1.ball != null) return 1;
            else if (IA_Player.ball != null) return 2;
        }
        return 0;
    }

    public GameObject FindWhoHasTheBall()
    {
        for (int i = 0; i < myPlayers.Length; i++)
        {
            MyPlayer_PVE player1 = myPlayers[i].GetComponent<MyPlayer_PVE>();
            MyPlayer_PVE IA_Player = myIA_Players[i].GetComponent<MyPlayer_PVE>();
            if (player1.ball != null) return player1.gameObject;
            else if (IA_Player.ball != null) return IA_Player.gameObject;
        }
        return null;
    }

    public void UpdateScoreBoard()
    {
        scoreBoard.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(score[0].ToString());
        scoreBoard.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(score[1].ToString());
    }

    public void Reposition()
    {
        GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().RepositionBall();
        for (int i = 0; i < myPlayers.Length; i++)
        {
            myPlayers[i].GetComponent<MyPlayer_PVE>().RepositionPlayer();
            myIA_Players[i].GetComponent<MyPlayer_PVE>().RepositionPlayer();
        }

        //GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.RPC("RepositionBall", RpcTarget.AllViaServer);
    }

    IEnumerator enableConfrontationAnim()
        {
        while (confontationAnimSprites.Count > 0) { yield return new WaitForSeconds(Time.deltaTime); }
        confontationAnimSprites.AddRange(field.GetComponentsInChildren<SpriteRenderer>(true));
        for(int i = 0; i < myPlayers.Length; i++)
        {
            if(i != fightingPlayer) confontationAnimSprites.AddRange(myPlayers[i].GetComponentsInChildren<SpriteRenderer>(true));
            if (i != fightingIA) confontationAnimSprites.AddRange(myIA_Players[i].GetComponentsInChildren<SpriteRenderer>(true));
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
        introObj.SetActive(true);
        introObj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text =
            introObj.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("username");

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
        if (GameObject.FindGameObjectWithTag("Ball").transform.GetChild(0).GetComponent<cameraMovement>().fingerIdx > idx)
            GameObject.FindGameObjectWithTag("Ball").transform.GetChild(0).GetComponent<cameraMovement>().fingerIdx--;
        //Manager
        if (fingerIdx > idx) fingerIdx--;

        //Players
        if (myPlayers[0].GetComponent<MyPlayer_PVE>().fingerIdx > idx) myPlayers[0].GetComponent<MyPlayer_PVE>().fingerIdx--;
        if (myPlayers[1].GetComponent<MyPlayer_PVE>().fingerIdx > idx) myPlayers[1].GetComponent<MyPlayer_PVE>().fingerIdx--;
        if (myPlayers[2].GetComponent<MyPlayer_PVE>().fingerIdx > idx) myPlayers[2].GetComponent<MyPlayer_PVE>().fingerIdx--;
    }

    public int getTotalTouches() { return touchesIdx.Count; }
}
