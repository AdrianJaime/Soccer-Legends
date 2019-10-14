using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Manager : MonoBehaviourPun, IPunObservable
{
    public GameObject player1Prefab, player2Prefab, ballPrefab, DirectionButtons;
    public bool GameStarted = false, GameOn = false;
    public MyPlayer player1, player2;
    private float timeStart = 0;
    private string fightDir, name1, name2;
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
                if (player1.fightDir == player2.fightDir) Fight(false, HasTheBall());
                GameOn = true; //Start Fight
                DirectionButtons.SetActive(false);
            }
            else if (fightDir != null)
            {
                player1.fightDir = fightDir;
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
    public void StartGame() { GameStarted = true; GameOn = true; }

    public void chooseDirection(MyPlayer _player1, MyPlayer _player2)
    {
        if (!DirectionButtons.activeSelf)
        {
            if (!_player2.colliding && _player2.playerObjective != Vector3.zero)
            {
                float[] arr = { _player1.transform.position.x, _player1.transform.position.y, _player1.transform.position.z};
                _player2.photonView.RPC("MoveTo", RpcTarget.AllViaServer, arr);
            }
            GameOn = false;
            DirectionButtons.SetActive(true);
            player1 = _player1;
            player2 = _player2;
        }
    }

    private void Fight(bool shoot, int ballPlayer)
    {
        if (shoot)
        {
            switch (ballPlayer)
            {
                case 1:
                    if (player1.stats.shoot >= player2.stats.defense) ; //GOAL player 1
                    break;
                case 2:
                    if (player2.stats.shoot >= player1.stats.defense) ; //GOAL player 2
                    break;
            }
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
    }

    public void setFightDir(string dir) { fightDir = dir; }

    private int HasTheBall()
    {
        if (player1.ball != null) return 1;
        else if (player2.ball != null) return 2;
        else return 0;
    }

}
