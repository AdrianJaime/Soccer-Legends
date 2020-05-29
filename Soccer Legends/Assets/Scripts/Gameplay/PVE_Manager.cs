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
    public enum fightState { FIGHT, SHOOT, NONE };

    public bool autoplay = false;
    public GameObject player1Prefab, player2Prefab, ballPrefab, directionSlide, specialSlide, scoreBoard, energyBar, timmer;
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
    [NonSerialized]
    public GameObject lastSpecialClip;
    private List<int> touchesIdx;
    private int fingerIdx = -1;
    private float enemySpecialBar = 0;
    private float energySegments = 0;

    private float timeStart = 0;
    public float fightRef = 0;
    private int fightingPlayer = 0, fightingIA = 0;
    private bool shooting = false;
    private Vector2 score = new Vector2(0, 0);
    private int randomValue;
    [System.NonSerialized]
    public fightState state = fightState.FIGHT;

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
    Image localType;
    [SerializeField]
    Image rivalType;

    [SerializeField]
    GameObject introObj;
    [SerializeField]
    Animator outroObj;
    [SerializeField]
    Text playerOutroPoints;
    [SerializeField]
    Text enemyOutroPoints;
    //Taps
    [SerializeField]
    Transform trailTap;
    public GameObject circleTapPrefab;

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
        swipes = new Vector2[] { Vector2.zero, Vector2.zero };
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
        if (!GameOn && GameStarted)
        {
           if(autoplay || (directionSlide.activeSelf && Input.touchCount == 1 && touchesIdx.Count == 0 || fingerIdx != -1))
            {
                Touch swipe = new Touch();
                if (!autoplay)
                {
                    if (fingerIdx != 0) fingerIdx = getTouchIdx();
                    try
                    {
                        swipe = Input.GetTouch(fingerIdx);
                    }
                    catch (ArgumentException e)
                    {
                        releaseTouchIdx(fingerIdx);
                        fingerIdx = -1;
                        return;
                    }
                }
                else { directionSlide.SetActive(false); specialSlide.SetActive(false); }
                if (!autoplay && swipe.phase == TouchPhase.Began)
                {
                    trailTap.gameObject.SetActive(true);
                    trailTap.position = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
                    trailTap.GetComponent<TrailRenderer>().enabled = true;
                }
                else if (!autoplay && swipe.phase == TouchPhase.Moved && swipes[1] == Vector2.zero)
                {
                    trailTap.position = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
                    if (trailTap.GetComponent<TrailRenderer>().positionCount > 0)
                    {
                        swipes[0] = trailTap.GetComponent<TrailRenderer>().GetPosition(0);
                        swipes[1] = trailTap.GetComponent<TrailRenderer>().positionCount >= 4 ? trailTap.GetComponent<TrailRenderer>()
                            .GetPosition(trailTap.GetComponent<TrailRenderer>().positionCount - 1) : Vector3.zero;
                    }
                }
                if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir == null && (autoplay || swipe.phase == TouchPhase.Ended))
                {
                    if (autoplay) swipes = new Vector2[] { Vector2.zero, Vector2.left };
                    if (swipes[1] != Vector2.zero)
                    {
                        if (state == fightState.FIGHT)
                        {
                            if (UnityEngine.Random.Range(0, 4) > 0 && myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo.specialAtack
                                .canUseSpecial(this, myIA_Players[fightingIA], enemySpecialBar * 5.0f))
                            {
                                myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = "Special";
                                enemySpecialBar -= myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo.requiredEnergy / 5.0f;
                                specialUpgrade(true);
                            }
                            else if (UnityEngine.Random.Range(0, 100) < (15 + (60 - ((int)(timeStart + 60 - Time.time)))))
                            {
                                myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = "Risky";
                                if (myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().ball != null)
                                    statsUpdate(true, 0, myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.technique / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2), 0);
                                else statsUpdate(true, 0, 0, myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.defense / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2));
                            }
                            else myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = "Normal";

                            if (swipes[0].y > swipes[1].y && Vector2.Angle(new Vector2(0, -1), new Vector2(swipes[1].x - swipes[0].x, swipes[1].y - swipes[0].y)) <= 60.0f && specialSlide.activeSelf)
                            {
                                myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Special";
                                float energy = energyBar.GetComponent<Slider>().value + energySegments;
                                energyBar.GetComponent<Slider>().value = energySegments = 0;
                                energy -= myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo.requiredEnergy;
                                while (energy > 1.0f) { energy -= 1.0f; energySegments++; }
                                energyBar.GetComponent<Slider>().value = energy;
                                specialUpgrade();
                            }
                            else if (swipes[0].x < swipes[1].x)
                            {
                                myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Risky";
                                if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().ball != null)
                                    statsUpdate(false, 0, myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.technique / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2), 0);
                                else statsUpdate(false, 0, 0, myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.defense / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2));
                            }
                            else myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Normal";
                        }
                        else if (state == fightState.SHOOT)
                        {
                            if (UnityEngine.Random.Range(0, 10) > 0 && myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo.specialAtack
                                .canUseSpecial(this, myIA_Players[fightingIA], enemySpecialBar * 5.0f))
                            {
                                myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = "Special";
                                enemySpecialBar -= myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo.requiredEnergy / 5.0f;
                                specialUpgrade(true);
                            }
                            else if (UnityEngine.Random.Range(0, 100) < (35 + (60 - ((int)(timeStart + 60 - Time.time)))))
                            {
                                myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = "Risky";
                                if (fightingIA == 3) statsUpdate(true, 0, 0, myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.defense / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2));
                                else statsUpdate(true, myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().stats.shoot / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2), 0, 0);
                            }
                            else myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = "Normal";

                            if (swipes[0].y > swipes[1].y && Vector2.Angle(new Vector2(0, -1), new Vector2(swipes[1].x - swipes[0].x, swipes[1].y - swipes[0].y)) <= 60.0f && specialSlide.activeSelf)
                            {
                                myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Special";
                                float energy = energyBar.GetComponent<Slider>().value + energySegments;
                                energyBar.GetComponent<Slider>().value = energySegments = 0;
                                energy -= myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo.requiredEnergy;
                                while (energy > 1.0f) { energy -= 1.0f; energySegments++; }
                                energyBar.GetComponent<Slider>().value = energy;
                                specialUpgrade();
                            }
                            else if (swipes[0].x < swipes[1].x)
                            {
                                myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Risky";
                                if (fightingPlayer == 3) statsUpdate(false, 0, 0, myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.defense / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2));
                                else statsUpdate(false, myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().stats.shoot / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2), 0, 0);
                            }
                            else myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Normal";
                        }
                        swipes = new Vector2[] { Vector2.zero, Vector2.zero };
                        directionSlide.SetActive(false); specialSlide.SetActive(false);
                    }
                    trailTap.GetComponent<TrailRenderer>().enabled = false;
                    releaseTouchIdx(fingerIdx);
                    fingerIdx = -1;
                }
            }
            if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir != null)
                Invoke("Fight", 0.5f);
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
            energyNumbers.GetComponent<TextMeshProUGUI>().SetText(energySegments.ToString());
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
        statsUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
        for (int i = 0; i < myIA_Players.Length; i++)
        {
            myPlayers[i].GetComponent<MyPlayer_PVE>().fightDir = null;
            myPlayers[i].GetComponent<MyPlayer_PVE>().SetStats();
            myIA_Players[i].GetComponent<MyPlayer_PVE>().fightDir = null;    
            myIA_Players[i].GetComponent<MyPlayer_PVE>().SetStats();
        }
        animator.ResetTrigger("Confrontation");
        animator.ResetTrigger("Battle");
        animator.ResetTrigger("Elude");
        animator.ResetTrigger("Lose");
        animator.ResetTrigger("Win");
        animator.ResetTrigger("SpecialAttack");
        animator.SetBool("SpecialAnim", false);
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
                state = fightState.FIGHT;
                if (player1.characterBasic.basicInfo.specialAttackInfo.specialAtack
                    .canUseSpecial(this, player1.gameObject, energyBar.GetComponent<Slider>().value + energySegments))
                    specialSlide.SetActive(true);
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
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "TEC "
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
                        playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique.ToString() + " TEC";
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "DEF "
                        + playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense.ToString();
                }
                updateUI_Stats();
                /*BONUS*/
                setPositionBonus(player1, IA_Player);
                setStrategyBonus(player1, IA_Player);
                setTypeBonus(player1, IA_Player);
                ///////////////////////////////////////
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
                state = fightState.SHOOT;
                if (player1.characterBasic.basicInfo.specialAttackInfo.specialAtack
                    .canUseSpecial(this, player1.gameObject, energyBar.GetComponent<Slider>().value + energySegments))
                    specialSlide.SetActive(true);
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
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "ATK "
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
                        playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot.ToString() + " ATK";
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "DEF "
                    + goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense.ToString();
            }
            updateUI_Stats();
            /*BONUS*/
            setPositionBonus(player1, IA_Player);
            setStrategyBonus(player1, IA_Player);
            setTypeBonus(player1, IA_Player);
            ////////////////////////////////////////
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
                {
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
                    if(playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique == playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense)
                        randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique + playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense + 1);
                    else if(playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique > playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense)
                        randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique + 1);
                    else randomValue = UnityEngine.Random.Range(playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique + 1, playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique + playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense + 1);
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
                    state = fightState.NONE;
                    setAnims(fightType, fightResult);
                }
            break;
            case fightState.SHOOT:
                {
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
                    if (playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot == goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense)
                        randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot + goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense + 1);
                    else if (playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot > goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense)
                        randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot + 1);
                    else randomValue = UnityEngine.Random.Range(playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot + 1, playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot + goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense + 1);
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
                case "TEC":
                    uiNumStat[i] = fightingPlayers[i].stats.technique;
                    break;
                case "ATK":
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

    void setTypeBonus(MyPlayer_PVE _p1, MyPlayer_PVE _p2)
    {
        List<KeyValuePair<MyPlayer_PVE, Image>> fightList = new List<KeyValuePair<MyPlayer_PVE, Image>>
        { new KeyValuePair<MyPlayer_PVE, Image>(_p1, localType),
            new KeyValuePair<MyPlayer_PVE, Image>(_p2, rivalType)};
        rivalType.GetComponent<RectTransform>().eulerAngles = localType.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        bool bonus = false;
        for (int i = 0; i < 2; i++) {
            Color c = new Color();
            switch (fightList[0].Key.characterBasic.basicInfo.type)
            {
                case Type.BLUE:
                    ColorUtility.TryParseHtmlString("#0092F8", out c);
                    bonus = fightList[1].Key.characterBasic.basicInfo.type == Type.RED;
                    break;
                case Type.GREEN:
                    ColorUtility.TryParseHtmlString("#19A600", out c);
                    bonus = fightList[1].Key.characterBasic.basicInfo.type == Type.BLUE;
                    break;
                case Type.PURPLE:
                    ColorUtility.TryParseHtmlString("#D700FF", out c);
                    bonus = fightList[1].Key.characterBasic.basicInfo.type == Type.GREEN;
                    break;
                case Type.RED:
                    ColorUtility.TryParseHtmlString("#D60000", out c);
                    bonus = fightList[1].Key.characterBasic.basicInfo.type == Type.YELLOW;
                    break;
                case Type.YELLOW:
                    ColorUtility.TryParseHtmlString("#E7E300", out c);
                    bonus = fightList[1].Key.characterBasic.basicInfo.type == Type.PURPLE;
                    break;
            }
            if(bonus)
            {
                MyPlayer_PVE.Stats statsBonus = new MyPlayer_PVE.Stats(0, 0, 0);
                if(state == fightState.SHOOT && fightList[0].Key.ball != null) statsBonus.shoot = fightList[0].Key.stats.shoot / 2;
                else if(fightList[0].Key.ball != null) statsBonus.technique = fightList[0].Key.stats.technique / 2;
                else if(fightList[0].Key.ball == null) statsBonus.defense = fightList[0].Key.stats.defense / 2;
                statsUpdate(!fightList[0].Key.transform.parent.GetComponent<IA_manager>().playerTeam, statsBonus.shoot, statsBonus.technique, statsBonus.defense);
                if(i == 0) rivalType.GetComponent<RectTransform>().eulerAngles = 
                        localType.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 90.0f);
                else rivalType.GetComponent<RectTransform>().eulerAngles =
                        localType.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 270.0f);
            }
            fightList[0].Value.color = c;
            fightList.Reverse();
        }
    }

    public void setStrategyBonus(MyPlayer_PVE _player1, MyPlayer_PVE _player2)
    {
        MyPlayer_PVE[] _players = new MyPlayer_PVE[] { _player1, _player2 };
        foreach(var _player in _players)
        switch (_player.transform.parent.GetComponent<IA_manager>().teamStrategy)
        {
            case IA_manager.strategy.DEFFENSIVE:
                statsUpdate(!_player.transform.parent.GetComponent<IA_manager>().playerTeam, 0, 0, _player.stats.defense * 20 / 100);
                break;
            case IA_manager.strategy.TECHNICAL:
                statsUpdate(!_player.transform.parent.GetComponent<IA_manager>().playerTeam, 0, _player.stats.technique * 20 / 100, 0);
                break;
            case IA_manager.strategy.OFFENSIVE:
                statsUpdate(!_player.transform.parent.GetComponent<IA_manager>().playerTeam, _player.stats.shoot * 20 / 100, 0, 0);
                break;
        }
    }

    void setPositionBonus(MyPlayer_PVE _player1, MyPlayer_PVE _player2)
    {
        MyPlayer_PVE[] _players = new MyPlayer_PVE[] { _player1, _player2 };
        foreach (var _player in _players)
        {
            bool hasBonus = false;
            switch (_player.formationPos)
            {
                case IA_manager.formationPositions.ALA:
                    hasBonus = _player.characterBasic.basicInfo.rol == Rol.WINGER;
                    break;
                case IA_manager.formationPositions.CIERRE:
                    hasBonus = _player.characterBasic.basicInfo.rol == Rol.LAST_MAN;
                    break;
                case IA_manager.formationPositions.GOALKEEPER:
                    hasBonus = _player.characterBasic.basicInfo.rol == Rol.GOALKEEPER;
                    break;
                case IA_manager.formationPositions.PIVOT:
                    hasBonus = _player.characterBasic.basicInfo.rol == Rol.PIVOT;
                    break;
            }
            if(hasBonus) statsUpdate(!_player.transform.parent.GetComponent<IA_manager>().playerTeam, 
                _player.stats.shoot * 10 / 100, _player.stats.technique * 10 / 100, _player.stats.defense * 10 / 100);
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

            //Override
            GameObject newAnimationClip = null;
            if (animator.GetBool("PlayerSpecial") && fightResult == "Win" && myPlayers[fightingPlayer]
                .GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo.specialClip != null)
                newAnimationClip = myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().characterBasic.basicInfo
                    .specialAttackInfo.specialClip;
            else if (animator.GetBool("EnemySpecial") && fightResult == "Lose" && myIA_Players[fightingIA]
                .GetComponent<MyPlayer_PVE>().characterBasic.basicInfo.specialAttackInfo.specialClip != null)
                newAnimationClip = myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().characterBasic.basicInfo
                    .specialAttackInfo.specialClip;
            if (newAnimationClip != null)
            {
                lastSpecialClip = Instantiate(newAnimationClip, animator.transform.parent);
                animator.SetBool("SpecialAnim", true);
                lastSpecialClip.SetActive(false);
            }
        }

        StartCoroutine(sliderEffect(waitTime, fightType, fightResult));
    }

    IEnumerator sliderEffect(float waitTime, string fightType, string fightResult)
    {
        yield return new WaitForSeconds(waitTime + Time.deltaTime);

        updateUI_Stats();

        statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<NumberEffect>().StartEffect();
        statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<NumberEffect>().StartEffect();

        Slider localS = statsUI.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>();
        Slider rivalS = statsUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>();
        trailTap.gameObject.SetActive(false);

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
        statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<NumberEffect>().enabled = false;
        statsUI.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<NumberEffect>().enabled = false;
        updateUI_Stats();
        localS.value = currentVal;
        rivalS.value = currentVal - localS.maxValue;
        localS.handleRect.GetComponent<Image>().enabled = currentVal <= localS.maxValue;
        rivalS.handleRect.GetComponent<Image>().enabled = currentVal - localS.maxValue > rivalS.minValue;

        //Set Results
        if(lastSpecialClip != null)lastSpecialClip.SetActive(animator.GetBool("SpecialAnim"));
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
        if (anim == "SpecialAnim")
        {
            anim = statsUI.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0)
                 .GetComponent<Slider>().handleRect.GetComponent<Image>().enabled == false ?
                 "EnemyWinConfrontation" : "PlayerWinBattle";
        }
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
                        playerWithBall.GetComponent<MyPlayer_PVE>().ShootBall(new float[] { -1.0f * (goalkeeper.transform.position.x / Mathf.Abs(goalkeeper.transform.position.x)), goalkeeper.GetComponent<MyPlayer_PVE>().rival_goal.transform.position.y, playerWithBall.GetComponent<MyPlayer_PVE>().ball.transform.position.x, playerWithBall.GetComponent<MyPlayer_PVE>().ball.transform.position.y });
                        goalRefFrame = Time.frameCount;
                    }
                    else
                    {
                        //Pierde el que chuta a puerta
                        goalkeeper = myPlayers[fightingPlayer];
                        playerWithBall = myIA_Players[fightingIA];
                        playerWithBall.GetComponent<MyPlayer_PVE>().Lose(true);
                    }
                }
                else //El jugador gana la batalla a la IA
                    myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().Lose();
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
                        playerWithBall.GetComponent<MyPlayer_PVE>().Lose(true);
                    }
                    else
                    {
                        //Gana el que chuta a puerta
                        goalkeeper = myPlayers[fightingPlayer];
                        playerWithBall = myIA_Players[fightingIA];
                        playerWithBall.GetComponent<MyPlayer_PVE>().ShootBall(new float[] { -1.0f * (goalkeeper.transform.position.x / Mathf.Abs(goalkeeper.transform.position.x)), goalkeeper.GetComponent<MyPlayer_PVE>().rival_goal.transform.position.y, playerWithBall.GetComponent<MyPlayer_PVE>().ball.transform.position.x, playerWithBall.GetComponent<MyPlayer_PVE>().ball.transform.position.y });
                        goalRefFrame = Time.frameCount;
                    }
                }
                else //La IA gana la batalla al jugador
                    myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().Lose();
                break;
            case "PlayerDodge":
            case "EnemyDodge":
                if (animator.GetBool("PlayerHasBall")) //El jugador esquiva a la IA
                    myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().Lose();
                else //La IA esquiva al jugador
                    myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().Lose();
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
        scoreBoard.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().SetText(score[0].ToString());
        scoreBoard.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().SetText(score[1].ToString());
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
        for (int i = 0; i < myPlayers.Length; i++)
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

        yield return new WaitForSeconds(0.25f);
        directionSlide.SetActive(true);

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
        introObj.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
            introObj.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = StaticInfo.tournamentTeam.teamName;
        introObj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text =
            introObj.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetString("username");

        yield return new WaitForSeconds(4.0f);
        StartGame();
        yield return new WaitForSeconds(2.0f);
        Destroy(introObj);
    }

    IEnumerator outro()
    {
        Time.timeScale = 1.0f;
        energyNumbers.transform.parent.GetComponent<strategyUI>().enabled = false;
        GameStarted = false;
        GameOn = false;
        outroObj.gameObject.SetActive(true);
        outroObj.SetTrigger("CallOutro");
        outroObj.SetBool("WIN", score[0] > score[1] ? true : false);
        playerOutroPoints.transform.GetChild(0).GetComponent<Text>().text = playerOutroPoints.text = score[0].ToString();
        enemyOutroPoints.transform.GetChild(0).GetComponent<Text>().text = enemyOutroPoints.text = score[1].ToString();

        yield return new WaitForSeconds(4.0f);
        SceneManager.LoadScene("ResultRewardScene");
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

    private Vector3 putZAxis(Vector3 vec) { return new Vector3(vec.x, vec.y, transform.position.z); }
}
