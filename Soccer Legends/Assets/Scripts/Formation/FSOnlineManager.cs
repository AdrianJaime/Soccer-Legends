using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class FSOnlineManager : MonoBehaviourPun
{
    [SerializeField]
    Formation formationScr;
    bool confirmTeam = false;
    bool rivalConfirmTeam = false;


    

    public void OnConfirmationClick()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("Player Count-> " + PhotonNetwork.CurrentRoom.PlayerCount.ToString());
            StaticInfo.teamSelectedToPlay.Clear();
            StaticInfo.teamSelectedToPlay.AddRange(formationScr.listOfCharactersInFormation);
            confirmTeam = true;
            string[] charactersName = new string[formationScr.listOfCharactersInFormation.Length];
            for (int i = 0; i < formationScr.listOfCharactersInFormation.Length; i++)
                charactersName[i] = formationScr.listOfCharactersInFormation[i].basicInfo.nameCharacter;
            photonView.RPC("getRivalConfirmation", RpcTarget.Others, charactersName);
            Destroy(GameObject.Find("Canvas"));
        }
    }

    [PunRPC]
    public void getRivalConfirmation(string[] _rivalTeam)
    {
        StaticInfo.rivalTeam = new List<CharacterBasic>();
        for(int i = 0; i < _rivalTeam.Length; i++)
        {
            foreach (CharacterBasic characterInfo in StaticInfo.teamSelectedToPlay)
            {
                if (characterInfo.basicInfo.nameCharacter == _rivalTeam[i]) StaticInfo.rivalTeam.Add(characterInfo);
            }
        }
        rivalConfirmTeam = true;
        if (rivalConfirmTeam && confirmTeam) photonView.RPC("LoadFormation", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void LoadFormation()
    {
        SceneManager.LoadScene("Match");
    }
}
