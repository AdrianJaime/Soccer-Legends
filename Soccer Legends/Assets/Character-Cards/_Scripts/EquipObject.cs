using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EquipObject : MonoBehaviour
{
    public int identifierEquip;

    public EquipCardLogic[] arraySlots = new EquipCardLogic[8];
    public CharacterBasic[] listOfCharacters = new CharacterBasic[8];

    public bool[] arrayEquiped = new bool[8];

    // public bool preferent;
    public int isUsed(CharacterBasic _aux)
    {
        for (int i = 0; i < 8; i++)
        {
            if (listOfCharacters[i].basicInfo.ID == _aux.basicInfo.ID)
                if (arrayEquiped[i] == true)
                    return i;
        }
        return -1;
    }




}
