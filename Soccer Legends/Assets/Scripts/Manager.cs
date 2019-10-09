﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Manager : MonoBehaviourPun, IPunObservable
{
    public GameObject player1Prefab, player2Prefab, ballPrefab;
    public bool GameStarted = false;
    private float timeStart = 0;
    private bool ball = false;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            if (Input.GetKeyDown(KeyCode.Space)) GameStarted = true;
        }
    }

    void SpawnPlayer()
    {
        
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.Instantiate(player2Prefab.name, player2Prefab.transform.position - new Vector3(0, 5, 0), player2Prefab.transform.rotation);
            PhotonNetwork.Instantiate(ballPrefab.name, new Vector3(0, 0, 0), ballPrefab.transform.rotation);
        }
        else PhotonNetwork.Instantiate(player1Prefab.name, player1Prefab.transform.position - new Vector3(0, 5, 0), player1Prefab.transform.rotation);
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
    public void StartGame() { GameStarted = true; }




}
