using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Manager : MonoBehaviourPun, IPunObservable
{
    public GameObject playerPrefab, ballPrefab;
    private float timeStart = 0;
    private bool ball = false;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("Ball")) ball = true;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            if ( timeStart == 0) timeStart = Time.time;
            if (Time.time - timeStart >= 10 && !ball)
            {
                PhotonNetwork.Instantiate(ballPrefab.name, Vector3.zero, ballPrefab.transform.rotation);
                ball = true;
            }
        }
    }

    void SpawnPlayer()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, playerPrefab.transform.position - new Vector3(0,5,0), playerPrefab.transform.rotation);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Send position to the other player. Stream == Getter.
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ball); //Solo se envía si se está moviendo.
        }
        else if (stream.IsReading)
        {
            ball = (bool)stream.ReceiveNext();
        }
    }




}
