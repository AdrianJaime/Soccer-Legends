using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_EquipInfo", menuName = "Equip/NewEquip")]
public class EquipData : ScriptableObject
{
    public CharacterInfo[] listOfCharacters =new CharacterInfo[8];

   // public bool preferent;
}
