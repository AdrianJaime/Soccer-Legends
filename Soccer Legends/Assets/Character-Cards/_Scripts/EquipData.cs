﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_EquipInfo", menuName = "Equip/NewEquip")]
public class EquipData : ScriptableObject
{
    public CharacterInfo[] listOfCharacters =new CharacterInfo[8];

    public bool[] arrayEquiped =new bool[8];

   // public bool preferent;
   public int isUsed(CharacterInfo _aux)
    {
        for(int i = 0; i < 8; i++)
        {
            if (listOfCharacters[i] == _aux)
                if (arrayEquiped[i] == true)
                    return i;
        }
        return -1;
    }
}
