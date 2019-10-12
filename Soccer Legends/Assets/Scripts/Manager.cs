using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Manager : MonoBehaviourPun, IPunObservable
{
    public GameObject player1Prefab, player2Prefab, ballPrefab, DirectionButtons;
    public bool GameStarted = false, GameOn = false;
    private float timeStart = 0;
    private string fightDir;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    private void Update()
    {
        if (!GameOn)
        {
            
        }
    }

    void SpawnPlayer()
    {

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            GameObject player = PhotonNetwork.Instantiate(player2Prefab.name, player2Prefab.transform.position - new Vector3(0, 5, 0), player2Prefab.transform.rotation);
            PhotonNetwork.Instantiate(ballPrefab.name, new Vector3(0, 0, 0), ballPrefab.transform.rotation);

            player.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = PhotonNetwork.LocalPlayer.NickName;
            Debug.Log(PhotonNetwork.LocalPlayer.IsMasterClient);
        }
        else
        {
            GameObject player = PhotonNetwork.Instantiate(player1Prefab.name, player1Prefab.transform.position - new Vector3(0, 5, 0), player1Prefab.transform.rotation);
            player.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = PhotonNetwork.LocalPlayer.NickName;
            Debug.Log(PhotonNetwork.LocalPlayer.IsMasterClient);
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

    public void chooseDirection(MyPlayer player1, MyPlayer player2)
    {
        if (player1.fightDir != null && player2.fightDir != null) GameOn = true; //Start Fight
        else if (!DirectionButtons.activeSelf)
        {
            GameOn = false;
            DirectionButtons.SetActive(true);
        }
    }

}
