using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_InventoryInfo", menuName = "Equip/NewInventory")]
public class InventoryData :ScriptableObject
{

    public List<CharacterInfo> listOfCharacters=new List<CharacterInfo>();



}
