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
    public enum fightState { FIGHT, SHOOT, NONE };

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
    [SerializeField]
    AnimationClip lastSpecialClip;
    private List<int> touchesIdx;
    private int fingerIdx = -1;
    private float enemySpecialBar = 0;
    public int energySegments = 0;
    public float energy;

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
    float tapRef;
    public GameObject circleTapPrefab;

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

        if (timeStart + 60 < Time.time || score.x == 3 || score.y == 3 || PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            photonView.RPC("FinishGame", RpcTarget.AllViaServer);
        }
        else
        {
            if (!GameOn) timeStart += Time.deltaTime;
            timmer.GetComponent<TextMeshProUGUI>().SetText(((int)(timeStart + 60 - Time.time)).ToString());
        }

        if (!GameOn && GameStarted)
        {
            if (fightRef + 7.5f < Time.time)
            {
                PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Normal";
                if (!PhotonNetwork.IsMasterClient)
                    photonView.RPC("setFightDir", RpcTarget.Others, 
                        PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir);
                directionSlide.SetActive(false); specialSlide.SetActive(false);
            }
            if (directionSlide.activeSelf && (Input.touchCount == 1 && touchesIdx.Count == 0 || fingerIdx != -1))
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
                    tapRef = Time.time;
                    swipes[0] = swipe.position;
                    trailTap.position = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
                }
                else if (swipe.phase == TouchPhase.Moved)
                {
                    trailTap.gameObject.SetActive(true);
                    trailTap.position = putZAxis(Camera.main.ScreenToWorldPoint(new Vector3(swipe.position.x, swipe.position.y, 0)));
                }
                else if (swipe.phase == TouchPhase.Ended && PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir == null)
                {
                    swipes[1] = swipe.position;
                    if (Vector2.Distance(swipes[0], swipes[1]) > Screen.width * 25.0f / 100.0f || tapRef + 0.5f < Time.time)
                    {
                        if (state == fightState.FIGHT)
                        {
                            if (swipes[0].y > swipes[1].y && Vector2.Angle(new Vector2(0, -1), new Vector2(swipes[1].x - swipes[0].x, swipes[1].y - swipes[0].y)) <= 60.0f && specialSlide.activeSelf)
                            {
                                PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Special";
                                float energy = energyBar.GetComponent<Slider>().value + energySegments;
                                energyBar.GetComponent<Slider>().value = energySegments = 0;
                                energy -= PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().characterBasic.basicInfo.specialAttackInfo.requiredEnergy;
                                while (energy > 1.0f) { energy -= 1.0f; energySegments++; }
                                energyBar.GetComponent<Slider>().value = energy;
                                photonView.RPC("specialUpgrade", RpcTarget.All, fightingPlayer);
                            }
                            else if (swipes[0].x < swipes[1].x)
                            {
                                PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Risky";
                                if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().ball != null)
                                    photonView.RPC("statsUpdate", RpcTarget.All, fightingPlayer, 0, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.technique / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2), 0);
                                else photonView.RPC("statsUpdate", RpcTarget.All, fightingPlayer, 0, 0, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.defense / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2));
                            }
                            else PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Normal";
                        }
                        else if (state == fightState.SHOOT)
                        {
                            if (swipes[0].y > swipes[1].y && Vector2.Angle(new Vector2(0, -1), new Vector2(swipes[1].x - swipes[0].x, swipes[1].y - swipes[0].y)) <= 60.0f && specialSlide.activeSelf)
                            {
                                PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Special";
                                float energy = energyBar.GetComponent<Slider>().value + energySegments;
                                energyBar.GetComponent<Slider>().value = energySegments = 0;
                                energy -= PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().characterBasic.basicInfo.specialAttackInfo.requiredEnergy;
                                while (energy > 1.0f) { energy -= 1.0f; energySegments++; }
                                energyBar.GetComponent<Slider>().value = energy;
                                photonView.RPC("specialUpgrade", RpcTarget.All, fightingPlayer);
                            }
                            else if (swipes[0].x < swipes[1].x)
                            {
                                PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Risky";
                                if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().formationPos == IA_manager.formationPositions.GOALKEEPER)
                                    photonView.RPC("statsUpdate", RpcTarget.All, fightingPlayer, 0, 0, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.defense / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2));
                                else photonView.RPC("statsUpdate", RpcTarget.All, fightingPlayer, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().stats.shoot / (UnityEngine.Random.Range(0, 2) == 0 ? -2 : 2), 0, 0);
                            }
                            else PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Normal";
                        }

                        photonView.RPC("setFightDir", RpcTarget.Others, PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir);

                        Debug.Log(PhotonView.Find(fightingPlayer).name + " from " + PhotonView.Find(fightingPlayer).transform.parent.name +
                        " chose direction " + PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir);
                        Debug.Log(PhotonView.Find(fightingIA).name + " from " + PhotonView.Find(fightingIA).transform.parent.name +
                            " chose direction " + PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir);
                        directionSlide.SetActive(false); specialSlide.SetActive(false);
                    }
                    trailTap.gameObject.SetActive(false);
                    releaseTouchIdx(fingerIdx);
                    fingerIdx = -1;
                }
            }
            if (PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir != null &&
            PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir != null && PhotonNetwork.IsMasterClient)
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
            energy = energyBar.GetComponent<Slider>().value + energySegments;
        }
    }


    void SpawnPlayers()
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            GameObject localPlayer = PhotonNetwork.Instantiate(player1Prefab.name, player1Prefab.transform.position - new Vector3(0, 5, 0), player1Prefab.transform.rotation);
            myPlayers = new GameObject[4];
            PhotonNetwork.Instantiate(ballPrefab.name, new Vector3(0, localPlayer.transform.position.y, 0), ballPrefab.transform.rotation);
            for (int i = 0; i < 4; i++)
            {
                myPlayers[i] = localPlayer.transform.GetChild(i).gameObject;
            }
        }
        else
        {
            GameObject localPlayer = PhotonNetwork.Instantiate(player2Prefab.name, player2Prefab.transform.position - new Vector3(0, 5, 0), player2Prefab.transform.rotation);
            myPlayers = new GameObject[4];
            for (int i = 0; i < 4; i++)
            {
                myPlayers[i] = localPlayer.transform.GetChild(i).gameObject;
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
    }

    [PunRPC]
    public void resumeGame()
    {
        GameStarted = true; GameOn = true;
        scoreBoard.SetActive(true); directionSlide.SetActive(false); specialSlide.SetActive(false); statsUI.SetActive(false);
        statsUI.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
        statsUI.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = 0;
        for (int i = 0; i < myIA_Players.Length; i++)
        {
            myPlayers[i].GetComponent<MyPlayer>().fightDir = null;
            myPlayers[i].GetComponent<MyPlayer>().SetStats();
            myIA_Players[i].GetComponent<MyPlayer>().fightDir = null;
            myIA_Players[i].GetComponent<MyPlayer>().SetStats();
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


    [PunRPC]
    public void FinishGame()
    {
        if(GameStarted)StartCoroutine(outro());
    }

    [PunRPC]
    public void chooseDirection(int _player1, int _player2)
    {
        if (!GameOn || PhotonView.Find(_player1).GetComponent<MyPlayer>().stunned || PhotonView.Find(_player2).GetComponent<MyPlayer>().stunned) return;
        if (myPlayers[0].GetComponent<MyPlayer>().photonView.Owner != PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).Owner)
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
                fightRef = Time.time;
                GameOn = false;
                state = fightState.FIGHT;
                if (player1.characterBasic.basicInfo.specialAttackInfo.specialAtack
                    .canUseSpecial(this, player1.gameObject, energyBar.GetComponent<Slider>().value + energySegments))
                    specialSlide.SetActive(true);
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
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "TEC "
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
                        playerWithBall.GetComponent<MyPlayer>().stats.technique.ToString() + " TEC";
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                    statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "DEF "
                        + playerWithoutBall.GetComponent<MyPlayer>().stats.defense.ToString();
                }
                updateUI_Stats();
                /*BONUS*/
                setPositionBonus(player1, IA_Player);
                setStrategyBonus(player1, IA_Player);
                setTypeBonus(player1, IA_Player);
                ///////////////////////////////////////
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
        if (myPlayers[0].GetComponent<MyPlayer>().photonView.Owner != PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).Owner)
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
        if (myPlayers[0].GetComponent<MyPlayer>().photonView.Owner != PhotonView.Find(GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.ViewID).Owner)
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
                fightRef = Time.time;
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
            if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().formationPos != IA_manager.formationPositions.GOALKEEPER)
            {
                playerWithBall = PhotonView.Find(fightingPlayer).gameObject;
                goalkeeper = PhotonView.Find(fightingIA).gameObject;
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "ATK "
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
                        playerWithBall.GetComponent<MyPlayer>().stats.shoot.ToString() + " ATK";
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
                statsUI.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = "DEF "
                    + goalkeeper.GetComponent<MyPlayer>().stats.defense.ToString();
            }
            updateUI_Stats();
            /*BONUS*/
            setPositionBonus(player1, IA_Player);
            setStrategyBonus(player1, IA_Player);
            setTypeBonus(player1, IA_Player);
            ////////////////////////////////////////
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
                if (playerWithBall.GetComponent<MyPlayer>().stats.technique == playerWithoutBall.GetComponent<MyPlayer>().stats.defense)
                    randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer>().stats.technique + playerWithoutBall.GetComponent<MyPlayer>().stats.defense + 1);
                else if (playerWithBall.GetComponent<MyPlayer>().stats.technique > playerWithoutBall.GetComponent<MyPlayer>().stats.defense)
                    randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer>().stats.technique + 1);
                else randomValue = UnityEngine.Random.Range(playerWithBall.GetComponent<MyPlayer>().stats.technique + 1, playerWithBall.GetComponent<MyPlayer>().stats.technique + playerWithoutBall.GetComponent<MyPlayer>().stats.defense + 1);
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
                
                photonView.RPC("setAnims", RpcTarget.AllViaServer, fightType, fightResult, randomValue);
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
                    if (playerWithBall.GetComponent<MyPlayer>().stats.shoot == goalkeeper.GetComponent<MyPlayer>().stats.defense)
                        randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer>().stats.shoot + goalkeeper.GetComponent<MyPlayer>().stats.defense + 1);
                    else if (playerWithBall.GetComponent<MyPlayer>().stats.shoot > goalkeeper.GetComponent<MyPlayer>().stats.defense)
                        randomValue = UnityEngine.Random.Range(1, playerWithBall.GetComponent<MyPlayer>().stats.shoot + 1);
                    else randomValue = UnityEngine.Random.Range(playerWithBall.GetComponent<MyPlayer>().stats.shoot + 1, playerWithBall.GetComponent<MyPlayer>().stats.shoot + goalkeeper.GetComponent<MyPlayer>().stats.defense + 1);
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
                photonView.RPC("setAnims", RpcTarget.AllViaServer, fightType, fightResult, randomValue);
                break;
            case fightState.NONE:
                return;

        }
        state = fightState.NONE;

    }

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
        GameObject specialOwner = PhotonView.Find(_id).gameObject, rival;
        SpecialAttackInfo specialInfo = specialOwner.GetComponent<MyPlayer>().characterBasic.basicInfo.specialAttackInfo;
        if (_id == fightingIA)
        {
            rival = PhotonView.Find(fightingPlayer).gameObject;
            iaSpecialName.text = specialInfo.name;
        }
        else
        {
            rival = PhotonView.Find(fightingIA).gameObject;
            mySpecialName.text = specialInfo.name;
        }
        StartCoroutine(specialInfo.specialAtack.callSpecial(this, specialOwner, rival));
    }

    void setTypeBonus(MyPlayer _p1, MyPlayer _p2)
    {
        List<KeyValuePair<MyPlayer, Image>> fightList = new List<KeyValuePair<MyPlayer, Image>>
        { new KeyValuePair<MyPlayer, Image>(_p1, localType),
            new KeyValuePair<MyPlayer, Image>(_p2, rivalType)};
        rivalType.GetComponent<RectTransform>().eulerAngles = localType.GetComponent<RectTransform>().eulerAngles = Vector3.zero;
        bool bonus = false;
        for (int i = 0; i < 2; i++)
        {
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
            if (bonus)
            {
                MyPlayer.Stats statsBonus = new MyPlayer.Stats(0, 0, 0);
                if (state == fightState.SHOOT && fightList[0].Key.ball != null) statsBonus.shoot = fightList[0].Key.stats.shoot / 2;
                else if (fightList[0].Key.ball != null) statsBonus.technique = fightList[0].Key.stats.technique / 2;
                else if (fightList[0].Key.ball == null) statsBonus.defense = fightList[0].Key.stats.defense / 2;
                statsUpdate(fightList[0].Key.photonView.ViewID, statsBonus.shoot, statsBonus.technique, statsBonus.defense);
                if (i == 0) rivalType.GetComponent<RectTransform>().eulerAngles =
                         localType.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 90.0f);
                else rivalType.GetComponent<RectTransform>().eulerAngles =
                        localType.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 270.0f);
            }
            fightList[0].Value.color = c;
            fightList.Reverse();
        }
    }

    public void setStrategyBonus(MyPlayer _player1, MyPlayer _player2)
    {
        MyPlayer[] _players = new MyPlayer[] { _player1, _player2 };
        foreach (var _player in _players)
            switch (_player.transform.parent.GetComponent<PVP_IA_manager>().teamStrategy)
            {
                case IA_manager.strategy.DEFFENSIVE:
                    statsUpdate(_player.photonView.ViewID, 0, 0, _player.stats.defense * 20 / 100);
                    break;
                case IA_manager.strategy.TECHNICAL:
                    statsUpdate(_player.photonView.ViewID, 0, _player.stats.technique * 20 / 100, 0);
                    break;
                case IA_manager.strategy.OFFENSIVE:
                    statsUpdate(_player.photonView.ViewID, _player.stats.shoot * 20 / 100, 0, 0);
                    break;
            }
    }

    void setPositionBonus(MyPlayer _player1, MyPlayer _player2)
    {
        MyPlayer[] _players = new MyPlayer[] { _player1, _player2 };
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
            if (hasBonus) statsUpdate(_player.photonView.ViewID,
                 _player.stats.shoot * 10 / 100, _player.stats.technique * 10 / 100, _player.stats.defense * 10 / 100);
        }
    }

    [PunRPC]
    public void setAnims(string fightType, string fightResult, int _randomValue)
    {
        if (!PhotonNetwork.IsMasterClient && fightType != "Elude")
        {
            fightResult = fightResult == "Win" ? "Lose" : "Win";
        }

        randomValue = _randomValue;

        //Set booleans
        animator.SetBool("PlayerHasBall", PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().ball != null ? true : false);
        animator.SetBool("PlayerSpecial", PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir == "Special");
        animator.SetBool("EnemySpecial", PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir == "Special");

        //Slider Effect waitTime 
        float waitTime = 0.0f;
        if (fightType == "SpecialAttack")
        {
            if (animator.GetBool("PlayerSpecial")) waitTime += 1.0f;
            if (animator.GetBool("EnemySpecial")) waitTime += 1.0f;
            animator.SetTrigger(fightType);

        //Override 
        AnimationClip newAnimationClip = null;
        if (animator.GetBool("PlayerSpecial") && fightResult == "Win" && PhotonView.Find(fightingPlayer)
            .GetComponent<MyPlayer>().characterBasic.basicInfo.specialAttackInfo.specialClip != null)
            newAnimationClip = PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().characterBasic.basicInfo
                .specialAttackInfo.specialClip;
        else if (animator.GetBool("EnemySpecial") && fightResult == "Lose" && PhotonView.Find(fightingIA)
            .GetComponent<MyPlayer>().characterBasic.basicInfo.specialAttackInfo.specialClip != null)
            newAnimationClip = PhotonView.Find(fightingIA).GetComponent<MyPlayer>().characterBasic.basicInfo
                .specialAttackInfo.specialClip;
        if (newAnimationClip != null)
        {
            AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);

            aoc[lastSpecialClip] = newAnimationClip;
            animator.runtimeAnimatorController = aoc;
            animator.runtimeAnimatorController.name = "OverrideRunTimeController";
            animator.SetBool("SpecialAnim", true);
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

        float sumMaxVal = localS.maxValue + rivalS.maxValue;
        float currentVal = localS.maxValue;
        float sumValue = Time.deltaTime * 3 * sumMaxVal;

        while (currentVal < sumMaxVal)
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
        animator.gameObject.GetComponent<Image>().enabled = animator.GetBool("SpecialAnim");
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
        if (anim == "SpecialAnim") anim = statsUI.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0) 
                 .GetComponent<Slider>().handleRect.GetComponent<Image>().enabled == false ? 
                 "EnemyWinConfrontation" : "PlayerWinBattle";
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
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, goalkeeper.GetComponent<MyPlayer>().photonView.ViewID, 0, 0, -goalkeeper.GetComponent<MyPlayer>().stats.defense / 3);
                        if (playerWithBall.GetComponent<MyPlayer>().fightDir == "Risky")
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, -playerWithBall.GetComponent<MyPlayer>().stats.shoot + playerWithBall.GetComponent<MyPlayer>().stats.shoot / 3, 0, 0);
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
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, goalkeeper.GetComponent<MyPlayer>().photonView.ViewID, 0, 0, -goalkeeper.GetComponent<MyPlayer>().stats.defense / 3);
                        if (playerWithBall.GetComponent<MyPlayer>().fightDir == "Risky")
                            photonView.RPC("statsUpdate", RpcTarget.AllViaServer, -playerWithBall.GetComponent<MyPlayer>().stats.shoot + playerWithBall.GetComponent<MyPlayer>().stats.shoot / 3, 0, 0);
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
        for (int i = 0; i < myPlayers.Length; i++)
        {
            MyPlayer player1 = myPlayers[i].GetComponent<MyPlayer>();
            MyPlayer IA_Player = myIA_Players[i].GetComponent<MyPlayer>();
            if (player1.ball != null) return 1;
            else if (IA_Player.ball != null) return 2;
        }
        Debug.Log("late 0");
        return 0;
    }

    public GameObject FindWhoHasTheBall()
    {
        for (int i = 0; i < myPlayers.Length; i++)
        {
            MyPlayer player1 = myPlayers[i].GetComponent<MyPlayer>();
            MyPlayer IA_Player = myIA_Players[i].GetComponent<MyPlayer>();
            if (player1.ball != null) return player1.gameObject;
            else if (IA_Player.ball != null) return IA_Player.gameObject;
        }
        return null;
    }

    [PunRPC]
    public void UpdateScoreBoard()
    {
        scoreBoard.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().SetText(score[0].ToString());
        scoreBoard.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().SetText(score[1].ToString());
        Debug.Log(score[0].ToString() + score[1].ToString());
    }

    [PunRPC]
    public void Reposition()
    {
        GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().RepositionBall();
        for (int i = 0; i < myPlayers.Length; i++)
        {
            myPlayers[i].GetComponent<MyPlayer>().RepositionPlayer();
            myIA_Players[i].GetComponent<MyPlayer>().RepositionPlayer();
        }
        //GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().photonView.RPC("RepositionBall", RpcTarget.AllViaServer);
    }

    IEnumerator enableConfrontationAnim()
    {
        while (confontationAnimSprites.Count > 0) { yield return new WaitForSeconds(Time.deltaTime); }
        confontationAnimSprites.AddRange(field.GetComponentsInChildren<SpriteRenderer>(true));
        for (int i = 0; i < myPlayers.Length; i++)
        {
            if (myPlayers[i] != PhotonView.Find(fightingPlayer).gameObject) confontationAnimSprites.AddRange(myPlayers[i].GetComponentsInChildren<SpriteRenderer>(true));
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
        while (GameObject.Find("Team 1(Clone)") == null || GameObject.Find("Team 2(Clone)") == null)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Transform rivalTeam = PhotonNetwork.IsMasterClient ? 
            GameObject.Find("Team 1(Clone)").transform : GameObject.Find("Team 2(Clone)").transform;

        myIA_Players = new GameObject[rivalTeam.childCount];

        for (int i = 0; i < 4; i++)
        {
            rivalTeam.GetChild(i).transform.position = myPlayers[i].transform.position * -1 + new Vector3(0, 1.0f, 0);
            myIA_Players[i] = rivalTeam.GetChild(i).gameObject;
        }

        introObj.SetActive(true);
        introObj.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text =
            introObj.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = PhotonNetwork.PlayerListOthers[0].NickName;
        introObj.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text =
            introObj.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = PhotonNetwork.LocalPlayer.NickName;


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
        outroObj.SetBool("WIN", score[0] > score[1] || PhotonNetwork.CurrentRoom.PlayerCount == 1 ? true : false);
        playerOutroPoints.transform.GetChild(0).GetComponent<Text>().text = playerOutroPoints.text = score[0].ToString();
        enemyOutroPoints.transform.GetChild(0).GetComponent<Text>().text = enemyOutroPoints.text = score[1].ToString();

        yield return new WaitForSeconds(4.0f);

        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
            Debug.Log("Disconnecting. . .");
        }
        Debug.Log("DISCONNECTED!");
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
        if (myPlayers[0].GetComponent<MyPlayer>().fingerIdx > idx) myPlayers[0].GetComponent<MyPlayer>().fingerIdx--;
        if (myPlayers[1].GetComponent<MyPlayer>().fingerIdx > idx) myPlayers[1].GetComponent<MyPlayer>().fingerIdx--;
        if (myPlayers[2].GetComponent<MyPlayer>().fingerIdx > idx) myPlayers[2].GetComponent<MyPlayer>().fingerIdx--;
    }

    public int getTotalTouches() { return touchesIdx.Count; }

    private Vector3 putZAxis(Vector3 vec) { return new Vector3(vec.x, vec.y, transform.position.z); }
}
