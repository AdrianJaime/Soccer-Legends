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
            string[] characterID = new string[formationScr.listOfCharactersInFormation.Length];
            int[] atq = new int[formationScr.listOfCharactersInFormation.Length];
            int[] teq = new int[formationScr.listOfCharactersInFormation.Length];
            int[] def = new int[formationScr.listOfCharactersInFormation.Length];
            for (int i = 0; i < formationScr.listOfCharactersInFormation.Length; i++)
            {
                characterID[i] = formationScr.listOfCharactersInFormation[i].basicInfo.ID;
                atq[i] = formationScr.listOfCharactersInFormation[i].info.atk;
                teq[i] = formationScr.listOfCharactersInFormation[i].info.teq;
                def[i] = formationScr.listOfCharactersInFormation[i].info.def;
            }
            photonView.RPC("getRivalConfirmation", RpcTarget.Others, characterID, atq, teq, def);
            Destroy(GameObject.Find("Confirm"));
            UnityEngine.UI.Button[] allBtn = GameObject.Find("Canvas").GetComponentsInChildren<UnityEngine.UI.Button>(true);
            foreach (UnityEngine.UI.Button _btn in allBtn) _btn.interactable = false;
        }
    }

    [PunRPC]
    public void getRivalConfirmation(string[] _ID, int[] _atq, int[] _teq, int[] _def)
    {
        StaticInfo.rivalTeam = new List<CharacterBasic>();
        for(int i = 0; i < _ID.Length; i++)
        {
            foreach (CharacterInfo characterInfo in formationScr.fullInventory.characters)
            {
                if (characterInfo.ID == _ID[i])
                   StaticInfo.rivalTeam.Add(new CharacterBasic(characterInfo, 
                       new CharacterBasic.data(_atq[i], _def[i], _teq[i])));
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
