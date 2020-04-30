using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Formation : MonoBehaviour
{
    public EquipCardFormationLogic[] slotsFormation = new EquipCardFormationLogic[4];
    public List<CharacterBasic> charactersAvailable;
    public CharactersCompendium fullInventory;
    public SceneManagerScript sceneManager;
    public Animator formationAnimator;
    public Image[] rivalSprites;
    public Transform localInfo, rivalInfo;

    [HideInInspector]
    public CharacterBasic[] listOfCharactersInFormation = new CharacterBasic[4];
    [HideInInspector]
    public bool[] arrayEquiped = new bool[4];

    [SerializeField] Button confirmButton;

    private void Awake()
    {
        //int a = 0;
        //if (StaticInfo.teamSelectedToPlay != null)
        //{
        //    foreach (CharacterBasic character in charactersAvailable)
        //    {
        //        if (StaticInfo.teamSelectedToPlay[a].basicInfo != null)
        //            character.basicInfo = StaticInfo.teamSelectedToPlay[a].basicInfo;
        //        a++;
        //    }
        //}
        charactersAvailable = StaticInfo.teamSelectedToPlay;
        //StaticInfo.teamSelectedToPlay.AddRange(listOfCharactersInFormation);
        for (int i = 0; i < StaticInfo.teamSelectedToPlay.Count; i++)
        {
            StaticInfo.teamSelectedToPlay[i].LoadCharacterStats(StaticInfo.teamSelectedToPlay[i].basicInfo.ID);
        }
    }

    public void CheckButtonInteractable()
    {
        confirmButton.interactable = (listOfCharactersInFormation[0] != null && 
                                      listOfCharactersInFormation[1] != null && 
                                      listOfCharactersInFormation[2] != null && 
                                      listOfCharactersInFormation[3] != null);
    }

    // public bool preferent;
    public int isUsed(CharacterBasic _aux)
    {
        int i = 0;
        foreach(CharacterBasic character in listOfCharactersInFormation)
        {
            if (character != null)
            {
                if (character.basicInfo.ID == _aux.basicInfo.ID)
                    if (arrayEquiped[i] == true)
                        return i;
            }
            i++;
        }
        return -1;
    }
     public void reorderTeam()
    {
        //StaticInfo.teamSelectedToPlay.Clear();
        //(StaticInfo.teamSelectedToPlay.AddRange(listOfCharactersInFormation);

        MyPlayer_PVE.Stats localStats = new MyPlayer_PVE.Stats(0,0,0), rivalStats = new MyPlayer_PVE.Stats(0,0,0);
        for (int i = 0; i < rivalSprites.Length; i++)
        {
            StaticInfo.teamSelectedToPlay[i].basicInfo = listOfCharactersInFormation[i].basicInfo;
            rivalSprites[i].sprite = StaticInfo.rivalTeam[i].basicInfo.artworkSelectorIcon;
            rivalStats.shoot += StaticInfo.rivalTeam[i].info.atk;
            localStats.shoot += StaticInfo.teamSelectedToPlay[i].info.atk;
            rivalStats.technique += StaticInfo.rivalTeam[i].info.teq;
            localStats.technique += StaticInfo.teamSelectedToPlay[i].info.teq;
            rivalStats.defense += StaticInfo.rivalTeam[i].info.def;
            localStats.defense += StaticInfo.teamSelectedToPlay[i].info.def;
        }

        localInfo.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetString("username");
        rivalInfo.GetChild(0).GetComponent<Text>().text = StaticInfo.tournamentTeam.teamName;
        localInfo.GetChild(1).GetComponent<Text>().text = "ATK: " + localStats.shoot.ToString();
        rivalInfo.GetChild(1).GetComponent<Text>().text = "ATK: " + rivalStats.shoot.ToString();
        localInfo.GetChild(2).GetComponent<Text>().text = "TEC: " + localStats.technique.ToString();
        rivalInfo.GetChild(2).GetComponent<Text>().text = "TEC: " + rivalStats.technique.ToString();
        localInfo.GetChild(3).GetComponent<Text>().text = "DEF: " + localStats.defense.ToString();
        rivalInfo.GetChild(3).GetComponent<Text>().text = "DEF: " + rivalStats.defense.ToString();

        formationAnimator.SetTrigger("AllReady");
        sceneManager.Invoke("LoadMatch", 4.0f);
    }
}
