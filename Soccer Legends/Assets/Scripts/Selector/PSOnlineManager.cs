using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PSOnlineManager : MonoBehaviourPun
{
    [SerializeField]
    PlayerSelector_PlayerManager mg;
    [SerializeField]
    GameObject connectScreen;
    bool confirmTeam = false;
    bool rivalConfirmTeam = false;

    private void Update()
    {
        connectScreen.SetActive(PhotonNetwork.CurrentRoom.PlayerCount != 2);
    }

    public void OnConfirmationClick()
    {
        Debug.Log("Player Count-> " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            confirmTeam = true;
            StaticInfo.teamSelectedToPlay = mg.characterSelected;
            photonView.RPC("getRivalConfirmation", RpcTarget.Others);
            Destroy(GameObject.Find("Canvas True"));
                }
    }

    [PunRPC]
    public void getRivalConfirmation()
    {
        rivalConfirmTeam = true;
        if(rivalConfirmTeam && confirmTeam) photonView.RPC("LoadFormation", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void LoadFormation()
    {
        SceneManager.LoadScene("FormationScene_PvP");
    }

}
