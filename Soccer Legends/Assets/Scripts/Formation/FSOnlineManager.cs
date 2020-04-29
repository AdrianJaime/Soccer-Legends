using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        for (int i = 0; i < _ID.Length; i++)
        {
            CharacterInfo auxCharacter = formationScr.fullInventory.compendiumOfCharacters.Find(x => x.ID == _ID[i]);
            StaticInfo.rivalTeam.Add(new CharacterBasic(auxCharacter,
                           new CharacterBasic.data(_atq[i], _def[i], _teq[i])));
        }

        rivalConfirmTeam = true;
        if (rivalConfirmTeam && confirmTeam) photonView.RPC("LoadFormation", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void LoadFormation()
    {
        MyPlayer.Stats localStats = new MyPlayer.Stats(0, 0, 0), rivalStats = new MyPlayer.Stats(0, 0, 0);
        for (int i = 0; i < formationScr.rivalSprites.Length; i++)
        {
            formationScr.rivalSprites[i].sprite = StaticInfo.rivalTeam[i].basicInfo.artworkSelectorIcon;
            rivalStats.shoot += StaticInfo.rivalTeam[i].info.atk;
            localStats.shoot += StaticInfo.teamSelectedToPlay[i].info.atk;
            rivalStats.technique += StaticInfo.rivalTeam[i].info.teq;
            localStats.technique += StaticInfo.teamSelectedToPlay[i].info.teq;
            rivalStats.defense += StaticInfo.rivalTeam[i].info.def;
            localStats.defense += StaticInfo.teamSelectedToPlay[i].info.def;
        }

        formationScr.localInfo.GetChild(0).GetComponent<Text>().text = PhotonNetwork.LocalPlayer.NickName;
        formationScr.rivalInfo.GetChild(0).GetComponent<Text>().text = PhotonNetwork.PlayerListOthers[0].NickName;
        formationScr.localInfo.GetChild(1).GetComponent<Text>().text = "ATK: " + localStats.shoot.ToString();
        formationScr.rivalInfo.GetChild(1).GetComponent<Text>().text = "ATK: " + rivalStats.shoot.ToString();
        formationScr.localInfo.GetChild(2).GetComponent<Text>().text = "TEC: " + localStats.technique.ToString();
        formationScr.rivalInfo.GetChild(2).GetComponent<Text>().text = "TEC: " + rivalStats.technique.ToString();
        formationScr.localInfo.GetChild(3).GetComponent<Text>().text = "DEF: " + localStats.defense.ToString();
        formationScr.rivalInfo.GetChild(3).GetComponent<Text>().text = "DEF: " + rivalStats.defense.ToString();

        formationScr.formationAnimator.SetTrigger("AllReady");
        formationScr.sceneManager.Invoke("LoadMatch", 4.0f);
    }
}
