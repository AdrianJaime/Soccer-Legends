using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Formation : MonoBehaviour
{
    public EquipCardFormationLogic[] slotsFormation = new EquipCardFormationLogic[4];
    public List<CharacterBasic> charactersAvailable;
    public CharactersCompendium fullInventory;

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
        StaticInfo.teamSelectedToPlay.Clear();
        StaticInfo.teamSelectedToPlay.AddRange(listOfCharactersInFormation);
    }
}
