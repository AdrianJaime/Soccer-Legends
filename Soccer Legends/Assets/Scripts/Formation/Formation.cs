using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{

    public EquipCardFormationLogic[] arraySlots = new EquipCardFormationLogic[4];
    public CharacterBasic[] listOfCharacters = new CharacterBasic[4];
    public bool[] arrayEquiped = new bool[4];

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

}
