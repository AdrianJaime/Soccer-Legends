using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName ="New_CharacterInventory", menuName ="CharacterInventory/NewInventory")]
public class CharacterInventory : ScriptableObject
{
   public CharacterInfo[] characters;
}
