using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class UIHandler : MonoBehaviourPunCallbacks
{
    public InputField createRoomTF, joinRoomTF, setNameTF;

    public void OnClick_JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        if (!PlayerPrefs.HasKey("username"))
            PlayerPrefs.SetString("username", "Unlogged");
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("username");
        print("Room Joined succesfully wuth name " + PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.LoadLevel("PlayersSelector_PvP");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("Random Room Failed" + returnCode + " Message " + message);
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
    }
} 
