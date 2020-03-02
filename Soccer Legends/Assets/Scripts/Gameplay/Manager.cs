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

    public GameObject player1Prefab, player2Prefab, ballPrefab, directionButtons, shootButtons, scoreBoard, startButton, energyBar;
    public bool GameStarted = false, GameOn = true;
    public GameObject[] myPlayers;
    public GameObject[] myIA_Players;
    public float eneregyFill;
    public GameObject lastPlayer;
    private List<int> touchesIdx;
    private int fingerIdx = -1;
    private float enemySpecialBar = 0;
    private int energySegments;

    private float timeStart = 0;
    private int fightingPlayer = 0, fightingIA = 0;
    private string fightDir;
    private bool shooting = false;
    private Vector2 score = new Vector2(0, 0);
    fightState state = fightState.FIGHT;

    Vector2[] swipes;

    //Hardcoded bug fixes
    int frameCount = 0;
    int goalRefFrame;

    // Start is called before the first frame update
    void Start()
    {
        timeStart = Time.time;
        touchesIdx = new List<int>();
        fingerIdx = -1;
        swipes = new Vector2[2];
        energySegments = energyBar.transform.GetChild(1).childCount - 1;
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

        if(goalRefFrame == Time.frameCount + 60) photonView.RPC("Goal", RpcTarget.AllViaServer);

        if (timeStart + 180 < Time.time || score.x == 5 || score.y == 5) SceneManager.LoadScene("MainMenuScene");

        if(!GameOn && GameStarted)
        {
            if ((Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.R)))
            {
                if(state == fightState.FIGHT)
                {
                    if (Input.GetKey(KeyCode.L)) PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Left";
                    else PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Right";
                }
                else if (state == fightState.SHOOT)
                {
                    if (Input.GetKey(KeyCode.L)) PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Special";
                    else PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Normal";
                }
                    Debug.Log(PhotonView.Find(fightingPlayer).name + " from " + PhotonView.Find(fightingPlayer).transform.parent.name +
                    " chose direction " + PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir);
                Debug.Log(PhotonView.Find(fightingIA).name + " from " + PhotonView.Find(fightingIA).transform.parent.name +
                    " chose direction " + PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir);

                Debug.Log(myIA_Players[0].GetComponent<MyPlayer>().fightDir);
                Debug.Log(myIA_Players[1].GetComponent<MyPlayer>().fightDir);
                Debug.Log(myIA_Players[2].GetComponent<MyPlayer>().fightDir);
                Debug.Log(myIA_Players[3].GetComponent<MyPlayer>().fightDir);

            }
            else if ((Input.touchCount == 1 && touchesIdx.Count == 0 || fingerIdx != -1))
            {
                if (fingerIdx != 0) fingerIdx = getTouchIdx();
                Touch swipe = Input.GetTouch(fingerIdx);
                if (swipe.phase == TouchPhase.Began)
                {
                    swipes[0] = swipe.position;
                }
                else if (swipe.phase == TouchPhase.Ended)
                {
                    swipes[1] = swipe.position;
                    if(state == fightState.FIGHT)
                    {
                        if (swipes[0].x > swipes[1].x) PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Left";
                        else PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Right";
                    }
                    else if(state == fightState.SHOOT)
                    {
                        if (swipes[0].x > swipes[1].x && energyBar.GetComponent<Scrollbar>().size * energySegments >= 1)
                        {
                            PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Special";
                            energyBar.GetComponent<Scrollbar>().size -= 1 / (float)energySegments;
                        }
                        else PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir = "Normal";
                    }
                    releaseTouchIdx(fingerIdx);
                    fingerIdx = -1;
                }
            }
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
            if (energyBar.GetComponent<Scrollbar>().size != 1) energyBar.GetComponent<Scrollbar>().size += (eneregyFill * Time.deltaTime) / energySegments;
            if (enemySpecialBar != 1) enemySpecialBar += (eneregyFill * Time.deltaTime) / energySegments;
        }
    }


    void SpawnPlayers()
    {

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            GameObject localPlayer = PhotonNetwork.Instantiate(player1Prefab.name, player1Prefab.transform.position - new Vector3(0, 5, 0), player1Prefab.transform.rotation);
            myPlayers = new GameObject[4];
            PhotonNetwork.Instantiate(ballPrefab.name, new Vector3(0, 0, 0), ballPrefab.transform.rotation);
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

    [PunRPC]
    public void StartGame() {
        GameStarted = true; GameOn = true;
        startButton.SetActive(false); scoreBoard.SetActive(true); directionButtons.SetActive(false); shootButtons.SetActive(false);
        GameObject T2 = GameObject.Find("Team 2(Clone)");
        GameObject T1 = GameObject.Find("Team 1(Clone)");
        myIA_Players = new GameObject[T1.transform.childCount];
        if (myPlayers[0].transform.parent.gameObject != T1)
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
        startButton.SetActive(false); scoreBoard.SetActive(true); directionButtons.SetActive(false); shootButtons.SetActive(false);
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
        if (player1.formationPos == IA_manager.formationPositions.GOALKEEPER)
        {
            player1.ball = GameObject.FindGameObjectWithTag("Ball");
            player1.ball.transform.localPosition = new Vector3(0, -0.5f, 0);
            IA_Player.GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer);
        }
        else if (IA_Player.formationPos == IA_manager.formationPositions.GOALKEEPER)
        {
            IA_Player.ball = GameObject.FindGameObjectWithTag("Ball");
            IA_Player.ball.transform.localPosition = new Vector3(0, -0.5f, 0);
            player1.GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer);
        }
        else if (!directionButtons.activeSelf)
        {
            if (HasTheBall() == 1)
            {
                float[] arr = { player1.transform.position.x, player1.transform.position.y, player1.transform.position.z };
                IA_Player.GetComponent<MyPlayer>().photonView.RPC("MoveTo", RpcTarget.AllViaServer, arr);
                player1.GetComponent<MyPlayer>().photonView.RPC("MoveTo", RpcTarget.AllViaServer, arr);
                //IA_Player.GetComponent<MyPlayer>().mg.photonView.RPC("chooseDirection", RpcTarget.AllViaServer, _player2, _player1);
            }
            GameOn = false;
            directionButtons.SetActive(true);
            state = fightState.FIGHT;
            //if(player1 == null) player1 = PhotonView.Find(_player1).gameObject.GetComponent<MyPlayer>();
            //if(IA_Player == null) IA_Player = PhotonView.Find(_player2).gameObject.GetComponent<MyPlayer>();
            player1.fightDir = null;
            IA_Player.fightDir = null;
            fightingPlayer = _player1;
            fightingIA = _player2;
            //GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

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

        if (!shootButtons.activeSelf)
        {
            GameOn = false;
            shootButtons.SetActive(true);
            state = fightState.SHOOT;
            //if(player1 == null) player1 = PhotonView.Find(_player1).gameObject.GetComponent<MyPlayer>();
            //if(IA_Player == null) IA_Player = PhotonView.Find(_player2).gameObject.GetComponent<MyPlayer>();
            player1.fightDir = null;
            IA_Player.fightDir = null;
            fightingPlayer = _player1;
            fightingIA = _player2;
            GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    private void Fight()
    {
        switch (state)
        {
            case fightState.FIGHT:
                //Debug.Log(myPlayers[fightingPlayer].name + " from " + myPlayers[fightingPlayer].transform.parent.name +
                //    " chose direction " + myPlayers[fightingPlayer].GetComponent<MyPlayer>().fightDir);
                //Debug.Log(myIA_Players[fightingIA].name + " from " + myIA_Players[fightingIA].transform.parent.name +
                //    " chose direction " + myIA_Players[fightingIA].GetComponent<MyPlayer>().fightDir);

             
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
                if (PhotonView.Find(fightingPlayer).GetComponent<MyPlayer>().fightDir == PhotonView.Find(fightingIA).GetComponent<MyPlayer>().fightDir)
                {
                    int randomValue = Random.Range(1, playerWithBall.GetComponent<MyPlayer>().stats.technique + playerWithoutBall.GetComponent<MyPlayer>().stats.defense + 1);
                    Debug.Log(playerWithBall.name + " from " + playerWithBall.transform.parent.name + "has a technique of " + playerWithBall.GetComponent<MyPlayer>().stats.technique.ToString() +
                    " and a range between 1 and " + playerWithBall.GetComponent<MyPlayer>().stats.technique.ToString());
                    Debug.Log(playerWithoutBall.name + " from " + playerWithoutBall.transform.parent.name + "has a deffense of " + playerWithoutBall.GetComponent<MyPlayer>().stats.defense.ToString() +
                    " and a range between " + (playerWithBall.GetComponent<MyPlayer>().stats.technique + 1).ToString() + " and " +
                    (playerWithBall.GetComponent<MyPlayer>().stats.technique +
                    playerWithoutBall.GetComponent<MyPlayer>().stats.defense).ToString());
                    Debug.Log("Random value-> " + randomValue.ToString());
                    if (randomValue > playerWithBall.GetComponent<MyPlayer>().stats.technique)
                    {
                        playerWithBall.GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer);
                    }
                    else playerWithoutBall.GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer);

                }
                else playerWithoutBall.GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer);
                break;
            case fightState.SHOOT:
                    //Debug.Log(myPlayers[fightingPlayer].name + " from " + myPlayers[fightingPlayer].transform.parent.name +
                    //    " chose shooting " + myPlayers[fightingPlayer].GetComponent<MyPlayer>().fightDir);
                    //Debug.Log(myIA_Players[fightingIA].name + " from " + myIA_Players[fightingIA].transform.parent.name +
                    //    " chose " + myIA_Players[fightingIA].GetComponent<MyPlayer>().fightDir);
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
                    if (playerWithBall.GetComponent<MyPlayer>().fightDir == goalkeeper.GetComponent<MyPlayer>().fightDir)
                    {
                        int randomValue = Random.Range(1, playerWithBall.GetComponent<MyPlayer>().stats.shoot + goalkeeper.GetComponent<MyPlayer>().stats.defense + 1);
                        Debug.Log(playerWithBall.name + " from " + playerWithBall.transform.parent.name + "has a shoot of " + playerWithBall.GetComponent<MyPlayer>().stats.shoot.ToString() +
                        " and a range between 1 and " + playerWithBall.GetComponent<MyPlayer>().stats.shoot.ToString());
                        Debug.Log(goalkeeper.name + " from " + goalkeeper.transform.parent.name + "has a deffense of " + goalkeeper.GetComponent<MyPlayer>().stats.defense.ToString() +
                        " and a range between " + (playerWithBall.GetComponent<MyPlayer>().stats.shoot + 1).ToString() + " and " +
                        (playerWithBall.GetComponent<MyPlayer>().stats.shoot +
                        goalkeeper.GetComponent<MyPlayer>().stats.defense).ToString());
                        Debug.Log("Random value-> " + randomValue.ToString());
                        if (randomValue <= playerWithBall.GetComponent<MyPlayer>().stats.shoot)
                        {
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
                            playerWithBall.GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer);
                    }
                    }
                    else
                    {
                    if (playerWithBall.GetComponent<MyPlayer>().fightDir == "Special")
                    {
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
                        playerWithBall.GetComponent<MyPlayer>().photonView.RPC("Lose", RpcTarget.AllViaServer);
                    }
                        energyBar.GetComponent<Scrollbar>().size -= 1 / (float)energySegments;
                    }
                    if (playerWithBall.GetComponent<MyPlayer>().fightDir == "Special") energyBar.GetComponent<Scrollbar>().size -= 1 / (float)energySegments;
                    if (goalkeeper.GetComponent<MyPlayer>().fightDir == "Special") enemySpecialBar -= 1 / (float)energySegments;
                break;
            case fightState.NONE:
                return;

        }
        state = fightState.NONE;

    }

    public void setFightDir(string dir) { fightDir = dir; }

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
        scoreBoard.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(score[0].ToString());
        scoreBoard.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(score[1].ToString());
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
        touchesIdx.Remove(idx);
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
}
