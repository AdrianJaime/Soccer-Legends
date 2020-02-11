using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PVE_Manager : MonoBehaviour
{
    enum fightState { FIGHT, SHOOT };

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
    private Vector2 score = new Vector2( 0, 0 );
    fightState state = fightState.FIGHT;

    Vector2[] swipes;

    //Hardcoded bug fixes
    int frameCount = 0;

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
        if (timeStart + 180 < Time.time || score.x == 5 || score.y == 5) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Debug.Log("Touches->" + touchesIdx.Count.ToString());
        if (!GameOn && (Input.touchCount == 1 && touchesIdx.Count == 0|| fingerIdx != -1))
        {
            switch(state)
            {
                case fightState.FIGHT:
                    myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = Random.Range(0, 2) == 0 ? "Left" : "Right";
                    if (fingerIdx != 0) fingerIdx = getTouchIdx();
                    Touch swipe = Input.GetTouch(fingerIdx);
                    if (swipe.phase == TouchPhase.Began)
                    {
                        swipes[0] = swipe.position;
                    }
                    else if(swipe.phase == TouchPhase.Moved)
                    {
                        swipes[1] = swipe.position;
                        if (swipes[0].x > swipes[1].x) myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Left";
                        else myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Right";

                        Debug.Log(myPlayers[fightingPlayer].name + " from " + myPlayers[fightingPlayer].transform.parent.name +
                            " chose direction " + myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir);
                        Debug.Log(myIA_Players[fightingIA].name + " from " + myIA_Players[fightingIA].transform.parent.name +
                            " chose direction " + myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir);

                        GameOn = true;
                        directionButtons.SetActive(false);
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
                        if (myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir == myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir)
                        {
                            int randomValue = Random.Range(1, playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique + playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense + 1);
                            Debug.Log(playerWithBall.name + " from " + playerWithBall.transform.parent.name + "has a technique of " + playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique.ToString() +
                            " and a range between 1 and " + playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique.ToString());
                            Debug.Log(playerWithoutBall.name + " from " + playerWithoutBall.transform.parent.name + "has a deffense of " + playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense.ToString() +
                            " and a range between " + (playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique + 1).ToString() + " and " +
                            (playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique + 
                            playerWithoutBall.GetComponent<MyPlayer_PVE>().stats.defense).ToString());
                            Debug.Log("Random value-> " + randomValue.ToString());
                            if (randomValue > playerWithBall.GetComponent<MyPlayer_PVE>().stats.technique)
                            {
                                playerWithBall.GetComponent<MyPlayer_PVE>().Lose();
                            }
                            else playerWithoutBall.GetComponent<MyPlayer_PVE>().Lose();

                        }
                        else playerWithoutBall.GetComponent<MyPlayer_PVE>().Lose();
                        releaseTouchIdx(fingerIdx);
                        fingerIdx = -1;
                    }      
                    break;
                case fightState.SHOOT:
                    myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir = Random.Range(0, 2) == 0 && enemySpecialBar * energySegments >= 1 ? "Special" : "Normal";
                    if (fingerIdx != 0) fingerIdx = getTouchIdx();
                    swipe = Input.GetTouch(fingerIdx);
                    if (swipe.phase == TouchPhase.Began)
                    {
                        swipes[0] = swipe.position;
                    }
                    else if (swipe.phase == TouchPhase.Moved)
                    {
                        swipes[1] = swipe.position;
                        if (swipes[0].x > swipes[1].x && energyBar.GetComponent<Scrollbar>().size * energySegments >= 1) myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Special";
                        else myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir = "Normal";

                        Debug.Log(myPlayers[fightingPlayer].name + " from " + myPlayers[fightingPlayer].transform.parent.name +
                            " chose shooting " + myPlayers[fightingPlayer].GetComponent<MyPlayer_PVE>().fightDir);
                        Debug.Log(myIA_Players[fightingIA].name + " from " + myIA_Players[fightingIA].transform.parent.name +
                            " chose " + myIA_Players[fightingIA].GetComponent<MyPlayer_PVE>().fightDir);

                        GameOn = true;
                        shootButtons.SetActive(false);
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
                        if (playerWithBall.GetComponent<MyPlayer_PVE>().fightDir == goalkeeper.GetComponent<MyPlayer_PVE>().fightDir)
                        {
                            int randomValue = Random.Range(1, playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot + goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense + 1);
                            Debug.Log(playerWithBall.name + " from " + playerWithBall.transform.parent.name + "has a shoot of " + playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot.ToString() +
                            " and a range between 1 and " + playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot.ToString());
                            Debug.Log(goalkeeper.name + " from " + goalkeeper.transform.parent.name + "has a deffense of " + goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense.ToString() +
                            " and a range between " + (playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot + 1).ToString() + " and " +
                            (playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot +
                            goalkeeper.GetComponent<MyPlayer_PVE>().stats.defense).ToString());
                            Debug.Log("Random value-> " + randomValue.ToString());          
                            if(randomValue <= playerWithBall.GetComponent<MyPlayer_PVE>().stats.shoot)
                            {
                                Goal(playerWithBall.transform.parent.name.Substring(0, 7) == "Team IA" ? false : true);
                            }
                            else
                            {
                                goalkeeper.GetComponent<MyPlayer_PVE>().GetBall();
                                playerWithBall.GetComponent<MyPlayer_PVE>().Lose(); 
                            }
                        }
                        else
                        {
                            if (playerWithBall.GetComponent<MyPlayer_PVE>().fightDir == "Special") Goal(playerWithBall.transform.parent.name.Substring(0, 7) == "Team IA" ? false : true);
                            else
                            {
                                goalkeeper.GetComponent<MyPlayer_PVE>().GetBall();
                                playerWithBall.GetComponent<MyPlayer_PVE>().Lose();
                            }
                            energyBar.GetComponent<Scrollbar>().size -= 1 / (float)energySegments;
                        }
                        if (playerWithBall.GetComponent<MyPlayer_PVE>().fightDir == "Special") energyBar.GetComponent<Scrollbar>().size -= 1 / (float)energySegments;
                        if (goalkeeper.GetComponent<MyPlayer_PVE>().fightDir == "Special") enemySpecialBar -= 1 / (float)energySegments;
                        releaseTouchIdx(fingerIdx);
                        fingerIdx = -1;
                    }
                    break;

            }
            //if (player1.fightDir != null && IA_Player.fightDir != null)
            //{
            //    if (player1.fightDir == "Normal" || player1.fightDir == "Special") Fight(true, HasTheBall());
            //    else if (player1.fightDir == IA_Player.fightDir) Fight(false, HasTheBall());
            //    else if (HasTheBall() == 1)
            //    {
            //        IA_Player.photonView.RPC("Lose", RpcTarget.AllViaServer);
            //        player1 = null;
            //        IA_Player = null;
            //    }
            //    else if (HasTheBall() == 2)
            //    {
            //        player1.photonView.RPC("Lose", RpcTarget.AllViaServer);
            //        player1 = null;
            //        IA_Player = null;
            //    }
            //    GameOn = true; //Start Fight
            //    directionButtons.SetActive(false);
            //    shootButtons.SetActive(false);
            //}
            //else if (fightDir != null)
            //{
            //    if (!shooting)
            //    {
            //        if (PhotonNetwork.IsMasterClient && player1.fightDir == null) player1.fightDir = fightDir;
            //        else if (!PhotonNetwork.IsMasterClient && IA_Player.fightDir == null) IA_Player.fightDir = fightDir;
            //        Debug.Log(player1.fightDir + IA_Player.fightDir);
            //    }
            //    else
            //    {
            //        if (PhotonNetwork.IsMasterClient && player1.fightDir == null)
            //        {
            //            if (fightDir == "Special" && energyBar.GetComponent<Scrollbar>().size == 1)
            //            {
            //                player1.fightDir = fightDir;
            //                energyBar.GetComponent<Scrollbar>().size = 0;
            //            }
            //            else if (fightDir == "Normal") player1.fightDir = fightDir;
            //        }
            //        else if (!PhotonNetwork.IsMasterClient && IA_Player.fightDir == null)
            //        {
            //            if (fightDir == "Special" && energyBar.GetComponent<Scrollbar>().size == 1)
            //            {
            //                IA_Player.fightDir = fightDir;
            //                energyBar.GetComponent<Scrollbar>().size = 0;
            //            }
            //            else if (fightDir == "Normal") IA_Player.fightDir = fightDir;
            //        }
            //    }
            //    fightDir = null;
            //}
        }
        else if (GameStarted && GameOn)
        {
            if(fingerIdx != -1)
            {
                releaseTouchIdx(fingerIdx);
                fingerIdx = -1;
            }
           if(energyBar.GetComponent<Scrollbar>().size != 1) energyBar.GetComponent<Scrollbar>().size += (eneregyFill * Time.deltaTime) / energySegments;
            if (enemySpecialBar != 1) enemySpecialBar += (eneregyFill * Time.deltaTime) / energySegments;
        }
    }


    void SpawnPlayers()
    {
        GameObject localPlayer = Instantiate(player1Prefab.gameObject, player1Prefab.transform.position - new Vector3(0, 5, 0), player1Prefab.transform.rotation);
        myPlayers = new GameObject[4];
        Instantiate(ballPrefab.gameObject, new Vector3(0, 0, 0), ballPrefab.transform.rotation);
        for (int i = 0; i < 4; i++)
        {
            myPlayers[i] = localPlayer.transform.GetChild(i).gameObject;
        }

        //IA Rival
        GameObject IA_Rival =  Instantiate(player2Prefab.gameObject, player2Prefab.transform.position + new Vector3(0, 5, 0), player2Prefab.transform.rotation);
        myIA_Players = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            IA_Rival.transform.GetChild(i).transform.position = localPlayer.transform.GetChild(i).transform.position * -1;
            myIA_Players[i] = IA_Rival.transform.GetChild(i).gameObject;
        }
        //player.transform.GetChild(0).GetComponent<MyPlayer>().photonView.RPC("SetName", RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer.NickName);       
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
        StartGame();
        releaseTouchIdx(fingerIdx);
        fingerIdx = -1;
    }

    public void StartGame() { GameStarted = true; GameOn = true; startButton.SetActive(false); scoreBoard.SetActive(true); }

    //public void chooseDirectionLoal(MyPlayer _player1, MyPlayer _player2)
    //{
    //    if (!directionButtons.activeSelf)
    //    {
    //        if (!_player2.colliding && _player2.playerObjective != Vector3.zero)
    //        {
    //            float[] arr = { _player1.transform.position.x, _player1.transform.position.y, _player1.transform.position.z};
    //            //_player2.photonView.RPC("MoveTo", RpcTarget.AllViaServer, arr);
    //        }
    //        GameOn = false;
    //        directionButtons.SetActive(true);
    //        player1 = _player1;
    //        IA_Player = _player2;
    //    }
    //}

    public void chooseDirection(int _player1, int _player2)
    {
        MyPlayer_PVE player1 = myPlayers[_player1].GetComponent<MyPlayer_PVE>();
        MyPlayer_PVE IA_Player = myIA_Players[_player2].GetComponent<MyPlayer_PVE>();
        if(player1.formationPos == IA_manager.formationPositions.GOALKEEPER)
        {
            player1.ball = GameObject.FindGameObjectWithTag("Ball");
            player1.ball.transform.localPosition = new Vector3(0, -0.5f, 0);
            IA_Player.GetComponent<MyPlayer_PVE>().Lose();
        }
        else if(IA_Player.formationPos == IA_manager.formationPositions.GOALKEEPER)
        {
            IA_Player.ball = GameObject.FindGameObjectWithTag("Ball");
            IA_Player.ball.transform.localPosition = new Vector3(0, -0.5f, 0);
            player1.GetComponent<MyPlayer_PVE>().Lose();
        }
        else if (!directionButtons.activeSelf)
        {
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

    public void Goal(bool isLocal)
    {
        if (isLocal) score[0]++;
        else score[1]++;

        lastPlayer = null;
        UpdateScoreBoard();
        Reposition();
        //photonView.RPC("UpdateScoreBoard", RpcTarget.AllViaServer);
        //photonView.RPC("Reposition", RpcTarget.AllViaServer);
    }


    public void ChooseShoot(int _player1, int _player2)
    {
        MyPlayer_PVE player1 = myPlayers[_player1].GetComponent<MyPlayer_PVE>();
        MyPlayer_PVE IA_Player = myIA_Players[_player2].GetComponent<MyPlayer_PVE>();
        if (!directionButtons.activeSelf)
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
        //MyPlayer_PVE player1;
        //MyPlayer_PVE IA_Player;
        //GameOn = false;
        //shootButtons.SetActive(true);
        ////if (player1 == null) player1 = PhotonView.Find(_player1).gameObject.GetComponent<MyPlayer>();
        ////if (IA_Player == null) IA_Player = PhotonView.Find(_player2).gameObject.GetComponent<MyPlayer>();
        //player1.fightDir = null;
        //IA_Player.fightDir = null;
        //shooting = true;
    }

    private void Fight(bool shoot, int ballPlayer)
    {
        //MyPlayer_PVE player1;
        //MyPlayer_PVE IA_Player;
        //if (shoot)
        //{
        //    if (player1.fightDir == "Special")
        //    {
        //        Debug.Log("1 special");
        //        player1.stats.shoot *= 10;
        //        player1.stats.defense *= 10;
        //    }

        //    if (IA_Player.fightDir == "Special")
        //    {
        //        Debug.Log("2 special");
        //        IA_Player.stats.shoot *= 10;
        //        IA_Player.stats.defense *= 10;
        //    }
        //    //switch (ballPlayer)
        //    //{
        //    //    case 1:
        //    //        if (player1.stats.shoot >= IA_Player.stats.defense && !player1.stunned && !IA_Player.stunned) photonView.RPC("Goal", RpcTarget.AllViaServer, true); //GOAL player 1
        //    //        else if (!player1.stunned && !IA_Player.stunned)
        //    //        {
        //    //            player1.photonView.RPC("Lose", RpcTarget.AllViaServer);
        //    //            IA_Player.photonView.RPC("GetBall", RpcTarget.AllViaServer);
        //    //        }
        //    //        break;
        //    //    case 2:
        //    //        if (IA_Player.stats.shoot >= player1.stats.defense && !player1.stunned && !IA_Player.stunned) photonView.RPC("Goal", RpcTarget.AllViaServer, false); //GOAL player 2
        //    //        else if (!player1.stunned && !IA_Player.stunned)
        //    //        {
        //    //            IA_Player.photonView.RPC("Lose", RpcTarget.AllViaServer);
        //    //            player1.photonView.RPC("GetBall", RpcTarget.AllViaServer);
        //    //        }
        //    //        break;
        //    //}
        //    shooting = false;
        //    if (player1.fightDir == "Special")
        //    {
        //        player1.stats.shoot /= 10;
        //        player1.stats.defense /= 10;
        //    }

        //    if (IA_Player.fightDir == "Special")
        //    {
        //        IA_Player.stats.shoot /= 10;
        //        IA_Player.stats.defense /= 10;
        //    }
        //}
        //else
        //{
        //    switch (ballPlayer)
        //    {
        //        case 1:
        //            Debug.Log("2:" + IA_Player.stats.defense);
        //            Debug.Log("1:" + player1.stats.technique);
        //            if (player1.stats.technique >= IA_Player.stats.defense) IA_Player.Lose(); //Ball player 1
        //            else if (!player1.stunned && !IA_Player.stunned)
        //            {
        //                //player1.photonView.RPC("Lose", RpcTarget.AllViaServer);
        //                //IA_Player.photonView.RPC("GetBall", RpcTarget.AllViaServer);
        //            }
        //            break;
        //        case 2:
        //            Debug.Log("1:" + player1.stats.defense);
        //            Debug.Log("2:" + IA_Player.stats.technique);
        //            if (IA_Player.stats.technique >= player1.stats.defense) player1.Lose(); //Ball player 2
        //            else if (!player1.stunned && !IA_Player.stunned)
        //            {
        //                //IA_Player.photonView.RPC("Lose", RpcTarget.AllViaServer);
        //                //player1.photonView.RPC("GetBall", RpcTarget.AllViaServer);
        //            }
        //            break;
        //    }
        //}
        //player1 = null;
        //IA_Player = null;
    }

    public void setFightDir(string dir) { fightDir = dir; }

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
        for (int i = 0; i < myPlayers.Length; i++)
        {
            myPlayers[i].GetComponent<MyPlayer_PVE>().RepositionPlayer();
            myIA_Players[i].GetComponent<MyPlayer_PVE>().RepositionPlayer();
        }
        GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().RepositionBall();
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
