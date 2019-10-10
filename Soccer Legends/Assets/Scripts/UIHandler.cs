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
        PhotonNetwork.JoinRoom(joinRoomTF.text, null);
    }
    public void OnClick_CreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoomTF.text, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);
    }

    public void setName()
    {
        PhotonNetwork.LocalPlayer.NickName = setNameTF.text;
    }
    public override void OnJoinedRoom()
    {
        print("Room Joined succesfully");
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Room Failed"+returnCode+" Message " +message);
    }
} 
