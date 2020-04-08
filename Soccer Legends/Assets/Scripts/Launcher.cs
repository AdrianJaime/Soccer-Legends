using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject connectScreen;
    [SerializeField]
    TextAlignment connectText;
    [SerializeField]
    GameObject returnObj;

    public void OnClick_ConnectBtn()
    {
        PhotonNetwork.ConnectUsingSettings(); //If succesfull OnConnectedToMaster is called.
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default); //If succesfull OnJoinedLobby is called.
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //Is called when disconneted, DisconnectCause tells us the reason it disconnected.
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        if (!PlayerPrefs.HasKey("username"))
            PlayerPrefs.SetString("username", "Unlogged");
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("username");
        print("Room Joined succesfully with name " + PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.LoadLevel("PlayersSelector_PvP");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("Random Room Failed" + returnCode + " Message " + message);
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
    }

    //public override void On... to cover all possibilities.
}
