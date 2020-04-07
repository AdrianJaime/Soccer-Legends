using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{

    public GameObject connectedScreen, disconnectedScreen;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //If succesfull OnConnectedToMaster is called.
    }

    private void Update()
    {
        if(connectedScreen.activeSelf)
        {
            connectedScreen.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>()
                .text = PlayerPrefs.GetString("username");
        }
    }
    public void OnClick_ConnectBtn()
    {
        PhotonNetwork.ConnectUsingSettings(); //If succesfull OnConnectedToMaster is called.
        if (disconnectedScreen.activeSelf)
            disconnectedScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default); //If succesfull OnJoinedLobby is called.
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (connectedScreen.activeSelf)
            connectedScreen.SetActive(false);
        disconnectedScreen.SetActive(true); //Is called when disconneted, DisconnectCause tells us the reason it disconnected.
    }

    public override void OnJoinedLobby()
    {
        if(disconnectedScreen.activeSelf)
            disconnectedScreen.SetActive(false);
        connectedScreen.SetActive(true);
    }

    //public override void On... to cover all possibilities.
}
