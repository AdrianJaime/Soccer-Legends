using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class MyPlayer : MonoBehaviourPun, IPunObservable
{
    public PhotonView pv;

    public float speed;

    private Vector3 smoothMove;

    public GameObject playerCamera;

    private void Start()
    {
        if (photonView.IsMine)
        {
            GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
            playerCamera.SetActive(true);
        }
    }
    private void Update()
    {
        if (photonView.IsMine) //Check if we're the local player
        {
            ProcessInputs();
        }
        else
        {
            smoothMovement();
        }
    }

    private void smoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
    }

    private void ProcessInputs()
    {
        //Movement
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Send position to the other player. Stream == Getter.
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position); //Solo se envía si se está moviendo.
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
        }
    }
}
