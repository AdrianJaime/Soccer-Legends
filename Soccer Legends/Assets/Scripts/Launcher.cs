using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject connectScreen;
    [SerializeField]
    Text connectText;
    [SerializeField]
    GameObject okButton;

    public void OnClick_ConnectBtn()
    {
        if(PhotonNetwork.IsConnected)
        {
            StartCoroutine(disconnetFromPhoton(true));
            return;
        }
        PhotonNetwork.ConnectUsingSettings(); //If succesfull OnConnectedToMaster is called.
        connectScreen.SetActive(true);
        connectText.text = "Connecting...";
        okButton.SetActive(false);
    }

    public void OnClick_OkBtn()
    {
        connectScreen.SetActive(false);
        okButton.SetActive(false);
    }

    public void OnClick_ReturnBtn()
    {
        if (PhotonNetwork.IsConnected)
        {
            StartCoroutine(disconnetFromPhoton(true));
            return;
        }
        connectScreen.SetActive(false);
        okButton.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default); //If succesfull OnJoinedLobby is called.
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //Is called when disconneted, DisconnectCause tells us the reason it disconnected.
        connectScreen.SetActive(true);
        connectText.text = "Connection Failed!";
        okButton.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
        connectScreen.SetActive(true);
        connectText.text = "Searching Match...";
        okButton.SetActive(false);
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

    IEnumerator disconnetFromPhoton(bool reconnect)
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
            Debug.Log("Disconnecting. . .");
        }
        Debug.Log("DISCONNECTED!");

        if (reconnect) OnClick_ConnectBtn();
        else OnClick_ReturnBtn();
    }
}
