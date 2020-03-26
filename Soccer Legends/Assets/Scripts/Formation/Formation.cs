using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Formation : MonoBehaviour
{
    public EquipCardFormationLogic[] arraySlots = new EquipCardFormationLogic[4];
    public List<CharacterBasic> arrayCharactersTeam;
    public CharacterBasic[] listOfCharacters = new CharacterBasic[4];
    public bool[] arrayEquiped = new bool[4];
    [SerializeField] Button confirmButton;

    private void Awake()
    {
        int a = 0;
        foreach(CharacterBasic character in arrayCharactersTeam)
        {
            if(StaticInfo.teamSelectedToPlay[a].basicInfo!=null)
                character.basicInfo = StaticInfo.teamSelectedToPlay[a].basicInfo;
            a++;
        }
    }

    private void Update()
    {
        confirmButton.interactable = (listOfCharacters[0] != null && listOfCharacters[1] != null && listOfCharacters[2] != null && listOfCharacters[3] != null);
    }

    // public bool preferent;
    public int isUsed(CharacterBasic _aux)
    {
        int i = 0;
        foreach(CharacterBasic character in listOfCharacters)
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
        StaticInfo.teamSelectedToPlay.AddRange(listOfCharacters);
    }
}
