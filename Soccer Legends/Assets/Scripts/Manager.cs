using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;

public class Manager : MonoBehaviourPun, IPunObservable
{
    public GameObject player1Prefab, player2Prefab, ballPrefab, directionButtons, shootButtons, scoreBoard, startButton;
    public bool GameStarted = false, GameOn = false;
    public MyPlayer player1, player2;

    private float timeStart = 0;
    private string fightDir;
    private bool shooting = false;
    private Vector2 score = new Vector2( 0, 0 );

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    private void Update()
    {
        if (!GameOn && player1 != null)
        {
            if (player1.fightDir != null && player2.fightDir != null)
            {
                if (player1.fightDir == "Normal" || player1.fightDir == "Special") Fight(true, HasTheBall());
                else if (player1.fightDir == player2.fightDir) Fight(false, HasTheBall());
                else if(HasTheBall() == 1) player2.photonView.RPC("Lose", RpcTarget.AllViaServer);
                else if(HasTheBall() == 2) player1.photonView.RPC("Lose", RpcTarget.AllViaServer);
                GameOn = true; //Start Fight
                directionButtons.SetActive(false);
                shootButtons.SetActive(false);
            }
            else if (fightDir != null)
            {
                if (!shooting) player1.fightDir = fightDir;
                else
                {
                    if (PhotonNetwork.IsMasterClient && player1.fightDir == null) player1.fightDir = fightDir;
                    else if(player2.fightDir == null) player2.fightDir = fightDir;
                }

                fightDir = null;
            }
        }
    }

    void SpawnPlayer()
    {

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            GameObject player = PhotonNetwork.Instantiate(player2Prefab.name, player2Prefab.transform.position - new Vector3(0, 5, 0), player2Prefab.transform.rotation);
            PhotonNetwork.Instantiate(ballPrefab.name, new Vector3(0, 0, 0), ballPrefab.transform.rotation);
            player.transform.GetChild(0).GetComponent<MyPlayer>().photonView.RPC("SetName", RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.NickName);
        }
        else
        {
            GameObject player = PhotonNetwork.Instantiate(player1Prefab.name, player1Prefab.transform.position - new Vector3(0, 5, 0), player1Prefab.transform.rotation);
            player.transform.GetChild(0).GetComponent<MyPlayer>().photonView.RPC("SetName", RpcTarget.AllBufferedViaServer, PhotonNetwork.LocalPlayer.NickName);
        }
        /* 
         {
             if (timeStart == 0) timeStart = Time.time;
             if (Time.time - timeStart >= 10 && !GameObject.FindGameObjectWithTag("Ball"))
             {

                 ball = true;
             }
         }*/
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Send position to the other player. Stream == Getter.
    {
        if (stream.IsWriting)
        {
            stream.SendNext(GameStarted); //Solo se envía si se está moviendo.
        }
        else if (stream.IsReading)
        {
            GameStarted = (bool)stream.ReceiveNext();
        }
    }

    public void StartButton()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) photonView.RPC("StartGame", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void StartGame() { GameStarted = true; GameOn = true; startButton.SetActive(false); scoreBoard.SetActive(true); }

    public void chooseDirection(MyPlayer _player1, MyPlayer _player2)
    {
        if (!directionButtons.activeSelf)
        {
            if (!_player2.colliding && _player2.playerObjective != Vector3.zero)
            {
                float[] arr = { _player1.transform.position.x, _player1.transform.position.y, _player1.transform.position.z};
                _player2.photonView.RPC("MoveTo", RpcTarget.AllViaServer, arr);
            }
            GameOn = false;
            directionButtons.SetActive(true);
            player1 = _player1;
            player2 = _player2;
        }
    }

    [PunRPC]
    public void Goal(bool isLocal)
    {
        if (isLocal) score[0]++;
        else score[1]++;

        UpdateScoreBoard();
        photonView.RPC("Reposition", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void ChooseShoot(int _player1, int _player2)
    {
        GameOn = false;
        shootButtons.SetActive(true);
        Debug.Log(_player1 + "    " + _player2);
        Debug.Log(PhotonView.Find(_player1).gameObject.name + "    " + PhotonView.Find(_player2).gameObject.name);
        if (player1 == null) player1 = PhotonView.Find(_player1).gameObject.GetComponent<MyPlayer>();
        if (player2 == null) player2 = PhotonView.Find(_player2).gameObject.GetComponent<MyPlayer>();
        player1.fightDir = null;
        player2.fightDir = null;
        shooting = true;
    }

    private void Fight(bool shoot, int ballPlayer)
    {
        Debug.Log("Chutamos, chabales");
        if (shoot)
        {
            Debug.Log("Chutamos, chabales");
            switch (ballPlayer)
            {
                case 1:
                    Debug.Log("Chutamos, chabales");
                    if (player1.stats.shoot >= player2.stats.defense) Goal(true); //GOAL player 1
                    else Debug.Log("Parada");
                    break;
                case 2:
                    Debug.Log("Chutamos, chabales");
                    if (player2.stats.shoot >= player1.stats.defense) Goal(false); //GOAL player 2
                    else Debug.Log("Parada");
                    break;
            }
            shooting = false;
        }
        else
        {
            switch (ballPlayer)
            {
                case 1:
                    Debug.Log("2:" + player2.stats.defense);
                    Debug.Log("1:" + player1.stats.technique);
                    if (player1.stats.technique >= player2.stats.defense) player2.Lose(); //Ball player 1
                    else if (!player1.stunned && !player2.stunned)
                    {
                        player1.photonView.RPC("Lose", RpcTarget.AllViaServer);
                        player2.photonView.RPC("GetBall", RpcTarget.AllViaServer);
                    }
                    break;
                case 2:
                    Debug.Log("1:" + player1.stats.defense);
                    Debug.Log("2:" + player2.stats.technique);
                    if (player2.stats.technique >= player1.stats.defense) player1.Lose(); //Ball player 2
                    else if (!player1.stunned && !player2.stunned)
                    {
                        player2.photonView.RPC("Lose", RpcTarget.AllViaServer);
                        player1.photonView.RPC("GetBall", RpcTarget.AllViaServer);
                    }
                    break;
            }
        }
        player1 = null;
        player2 = null;
    }

    public void setFightDir(string dir) { fightDir = dir; }

    private int HasTheBall()
    {
        if (player1.ball != null) return 1;
        else if (player2.ball != null) return 2;
        else return 0;
    }

    private void UpdateScoreBoard()
    {
        scoreBoard.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(score[0].ToString());
        scoreBoard.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().SetText(score[1].ToString());
    }

    [PunRPC]
    private void Reposition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++) players[i].transform.position = players[i].GetComponent<MyPlayer>().startPosition;
        GameObject.FindGameObjectWithTag("Ball").GetComponent<Ball>().Reposition();
    }
}
